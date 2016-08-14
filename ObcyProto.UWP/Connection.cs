using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using ObcyProto.UWP.Client;
using ObcyProto.UWP.Client.Identity;
using ObcyProto.UWP.Client.Packets;
using ObcyProto.UWP.Events;
using ObcyProto.UWP.Server.Packets;
using ObcyProto.UWP.SockJS;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace ObcyProto.UWP
{
    public class Connection
    {
        private SocketTargetAddress SocketTargetAddress { get; set; }
        private MessageWebSocket WebSocket { get; set; }
        private DataWriter WebSocketWriter { get; set; }

        internal static int ActionID = 1;

        public bool IsOpen { get; private set; }
        public bool IsSearchingForStranger { get; private set; }
        public bool IsStrangerConnected { get; private set; }
        public bool KeepAlive { get; set; }
        public bool IsReady { get; private set; }
        public bool SendUserAgent { get; set; }

        public UserAgent UserAgent { get; set; }

        private List<string> EncounteredClientIDs { get; }
        public string CurrentClientID { get; private set; }
        public string CurrentContactUID { get; private set; }

        public event EventHandler ConnectionOpened;
        public event EventHandler<ConnectionAcceptedEventArgs> ConnectionAccepted;
        public event EventHandler<ConversationEndedEventArgs> ConversationEnded;
        public event EventHandler<DisconnectEventArgs> ConnectionClosed;
        public event EventHandler<ErrorEventArgs> ErrorOccured;
        public event EventHandler<HeartbeatEventArgs> HeartbeatReceived;
        public event EventHandler<JsonEventArgs> JsonRead;
        public event EventHandler<JsonEventArgs> JsonWrite;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<MessageEventArgs> MessageSent;
        public event EventHandler<OnlineCountEventArgs> OnlinePeopleCountChanged;
        public event EventHandler<PingEventArgs> PingReceived;
        public event EventHandler ServerClosedConnection;
        public event EventHandler<ChatstateEventArgs> StrangerChatstateChanged;
        public event EventHandler<StrangerFoundEventArgs> StrangerFound;

        public Connection()
        {
            SetupWebSocket(); 
            RegisterPacketHandlerEvents();

            KeepAlive = true;
            IsReady = true;
            SendUserAgent = true;

            UserAgent = new UserAgent("", "2.5");

            EncounteredClientIDs = new List<string>();
        }

        public async Task ConnectAsync()
        {
            if (IsReady && !IsOpen)
            {
                try
                {
                    await WebSocket.ConnectAsync(new Uri(SocketTargetAddress));
                    WebSocketWriter = new DataWriter(WebSocket.OutputStream);

                    IsOpen = true;
                    ConnectionOpened?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    ErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
                }
            }
        }

        public void Disconnect(ushort code, string reason)
        {
            if (IsOpen)
            {
                WebSocket.Close(code, reason);

                IsOpen = false;
                IsReady = false;
                ConnectionClosed?.Invoke(this, new DisconnectEventArgs(code, reason));
            }
        }

        public async void EndConversation()
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                await SendPacketAsync(new DisconnectPacket(CurrentContactUID));
                IsStrangerConnected = false;

                var di = new DisconnectInfo(false, -2);
                var eventArgs = new ConversationEndedEventArgs(di);
                ConversationEnded?.Invoke(this, eventArgs);

                ActionID++;
            }
        }

        public async void FlagStranger()
        {
            if (IsOpen && IsReady && IsStrangerConnected)
            {
                await SendPacketAsync(new ReportStrangerPacket(CurrentContactUID));
                ActionID++;
            }
        }

        public async void SendChatstate(bool typing)
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                await SendPacketAsync(new ChatstatePacket(typing, CurrentContactUID));
            }
        }

        public async void RequestRandomTopic()
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                await SendPacketAsync(new RandomTopicPacket(CurrentContactUID));
                ActionID++;
            }
        }

        public async void SearchForStranger(Location targetLocation)
        {
            if (IsReady && IsOpen && !IsStrangerConnected)
            {
                if (!IsSearchingForStranger)
                {
                    IsSearchingForStranger = true;

                    var info = new PersonInfo(0, targetLocation);
                    await SendPacketAsync(new StrangerSearchPacket(info, info, "main"));
                }
                ActionID++;
            }
        }

        public async void SendMessage(string message)
        {
            if (IsReady && IsOpen && IsStrangerConnected)
            {
                await SendPacketAsync(new MessagePacket(message, CurrentContactUID));

                var eventArgs = new MessageEventArgs(new Message(message, -1, -1 , MessageType.Chat));
                MessageSent?.Invoke(this, eventArgs);
                ActionID++;
            }
        }

        public async void SendPong()
        {
            if (IsOpen && IsReady)
            {
                await SendPacketAsync(new PongPacket());
            }
        }   

        public async Task SendPacketAsync(Packet packet)
        {
            await SendJsonAsync(packet);
        }

        public async Task SendJsonAsync(string json)
        {
            if (IsReady && IsOpen)
            {
                WebSocketWriter.WriteString(json);

                try
                {
                    await WebSocketWriter.StoreAsync();
                    JsonWrite?.Invoke(this, new JsonEventArgs(json));
                }
                catch(Exception ex)
                {
                    ErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
                }
            }
        }

        private void SetupWebSocket()
        {
            Disconnect(0, "");
            SocketTargetAddress = new SocketTargetAddress();

            WebSocket = new MessageWebSocket();
            WebSocket.Control.MessageType = SocketMessageType.Utf8;

            WebSocket.SetRequestHeader("Origin", SocketTargetAddress.Origin);
            WebSocket.MessageReceived += WebSocket_MessageReceived;
            WebSocket.Closed += WebSocket_Closed;
        }

        private void RegisterPacketHandlerEvents()
        {
            PacketHandler.SocketMessageReceived += PacketHandler_SocketMessageReceived;
            PacketHandler.ConnectionClosePacketReceived += PacketHandler_ConnectionClosePacketReceived;
            PacketHandler.SocketHeartbeatReceived += PacketHandler_SocketHeartbeatReceived;
        }

        private void PacketHandler_SocketHeartbeatReceived(DateTime heartbeatTime)
        {
            HeartbeatReceived?.Invoke(this, new HeartbeatEventArgs(heartbeatTime));
        }

        private void PacketHandler_ConnectionClosePacketReceived(EventArgs e)
        {
            ServerClosedConnection?.Invoke(this, EventArgs.Empty);
        }

        private async void PacketHandler_SocketMessageReceived(List<Packet> packets)
        {
            // Server may send more than one packet.
            // -------------------------------------
            foreach (var packet in packets)
            {
                if (packet.Header == ConnectionAcceptedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    await SendPacketAsync(new ClientInfoPacket(false, UserAgent, packet.Data["hash"].ToString(), 0, false));
                    await SendPacketAsync(new OpenAcknowledgedPacket());

                    var eventArgs = new ConnectionAcceptedEventArgs(packet.Data["conn_id"].ToString(), packet.Data["hash"].ToString());
                    ConnectionAccepted?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == ConversationEndedPacket.ToString())
                {
                    // Unusual behavior, server sends "convended" without any data
                    // if "flag stranger" packet is sent and no conversation have
                    // been started before.
                    //
                    // Hence, we have to handle it like this.
                    // -----------------------------------------------------------
                    IsStrangerConnected = false;
                    if (packet.Data != null)
                    {
                        var di = new DisconnectInfo(true, int.Parse(packet.Data.ToString()));
                        var eventArgs = new ConversationEndedEventArgs(di);

                        ConversationEnded?.Invoke(this, eventArgs);
                    }
                    else
                    {
                        var di = new DisconnectInfo(true, -1);
                        var eventArgs = new ConversationEndedEventArgs(di);

                        ConversationEnded?.Invoke(this, eventArgs);
                    }
                    continue;
                }

                if (packet.Header == StrangerDisconnectedPacket.ToString())
                {
                    if (CurrentClientID != packet.Data.ToString() && EncounteredClientIDs.Contains(packet.Data.ToString()))
                    {
                        EncounteredClientIDs.Remove(packet.Data.ToString());
                        continue;
                    }

                    IsStrangerConnected = false;

                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    var di = new DisconnectInfo(false, int.Parse(packet.Data.ToString()));
                    var eventArgs = new ConversationEndedEventArgs(di);

                    ConversationEnded?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == MessageReceivedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    int postId = -1;
                    if (packet.AdditionalFields.ContainsKey("post_id"))
                        postId = int.Parse(packet.AdditionalFields["post_id"].ToString());

                    var message = new Message(
                        packet.Data["msg"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        postId,
                        MessageType.Chat
                    );
                    var eventArgs = new MessageEventArgs(message);
                    MessageReceived?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == OnlinePeopleCountPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    int number;
                    if (!int.TryParse(packet.Data.ToString(), out number))
                    {
                        number = -1;
                    }

                    var eventArgs = new OnlineCountEventArgs(number);
                    OnlinePeopleCountChanged?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == PingPacket.ToString())
                {
                    if (KeepAlive)
                        SendPong();

                    var eventArgs = new PingEventArgs(DateTime.Now);
                    PingReceived?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == RandomTopicReceivedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    var message = new Message(
                        packet.Data["topic"].ToString(),
                        int.Parse(packet.Data["cid"].ToString()),
                        int.Parse(packet.AdditionalFields["post_id"].ToString()),
                        MessageType.Topic
                    );
                    var eventArgs = new MessageEventArgs(message);
                    MessageReceived?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == ServiceMessageReceivedPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    var message = new Message(packet.Data.ToString(), -1, -1, MessageType.Service);
                    var eventArgs = new MessageEventArgs(message);
                    MessageReceived?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == StrangerChatstatePacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    bool writing;
                    if (!bool.TryParse(packet.Data.ToString(), out writing))
                    {
                        writing = false;
                    }

                    var chatState = writing ? ChatState.Typing : ChatState.Idle;
                    var eventArgs = new ChatstateEventArgs(chatState);

                    StrangerChatstateChanged?.Invoke(this, eventArgs);
                    continue;
                }

                if (packet.Header == StrangerFoundPacket.ToString())
                {
                    if (packet.Data == null)
                        throw new Exception("Invalid packet received, packet data is null.");

                    CurrentContactUID = packet.Data["ckey"].ToString();

                    await SendPacketAsync(new ConversationStartAcknowledgedPacket(CurrentContactUID));
                    ActionID++;

                    EncounteredClientIDs.Add(packet.Data["cid"].ToString());
                    CurrentClientID = packet.Data["cid"].ToString();

                    IsSearchingForStranger = false;
                    IsStrangerConnected = true;

                    var si = new StrangerInfo(
                        int.Parse(packet.Data["cid"].ToString()),
                        packet.Data["ckey"].ToString(),
                        bool.Parse(packet.Data["flaged"].ToString()),
                        packet.Data["info"]
                    );

                    var eventArgs = new StrangerFoundEventArgs(si);
                    StrangerFound?.Invoke(this, eventArgs);
                }
            }
        }

        private void WebSocket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            using (var reader = args.GetDataReader())
            {
                reader.UnicodeEncoding = UnicodeEncoding.Utf8;

                try
                {
                    string data = reader.ReadString(reader.UnconsumedBufferLength);

                    JsonRead?.Invoke(this, new JsonEventArgs(data));
                    PacketHandler.HandlePacket(data);
                }
                catch (Exception ex)
                {
                    ErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
                }
            }
        }

        private void WebSocket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            ConnectionClosed?.Invoke(this, new DisconnectEventArgs(args.Code, args.Reason));
        }
    }
}

using Newtonsoft.Json.Linq;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class MessagePacket : Packet
    {
        public MessagePacket(string body, string strangerUid)
        {
            Header = "_pmsg";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
                ["msg"] = body,
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}

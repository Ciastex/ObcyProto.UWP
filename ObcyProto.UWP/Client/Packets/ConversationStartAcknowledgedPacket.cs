using Newtonsoft.Json.Linq;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class ConversationStartAcknowledgedPacket : Packet
    {
        public ConversationStartAcknowledgedPacket(string strangerUid)
        {
            Header = "_begacked";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}

using Newtonsoft.Json.Linq;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class ReportStrangerPacket : Packet
    {
        public ReportStrangerPacket(string strangerUid)
        {
            Header = "_reptalk";

            Data = new JObject
            {
                ["ckey"] = strangerUid
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}

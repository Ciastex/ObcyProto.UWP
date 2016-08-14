using Newtonsoft.Json.Linq;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class RandomTopicPacket : Packet
    {
        public RandomTopicPacket(string strangerUid)
        {
            Header = "_randtopic";

            Data = new JObject
            {
                ["ckey"] = strangerUid
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}

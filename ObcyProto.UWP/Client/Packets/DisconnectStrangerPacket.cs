using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class DisconnectPacket : Packet
    {
        public DisconnectPacket(string strangerUid)
        {
            Header = "_distalk";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
            };
            base["ceid"] = Connection.ActionID;
        }
    }
}

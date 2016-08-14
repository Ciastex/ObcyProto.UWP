using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class ChatstatePacket : Packet
    {
        public ChatstatePacket(bool typing, string strangerUid)
        {
            Header = "_mtyp";

            Data = new JObject
            {
                ["ckey"] = strangerUid,
                ["val"] = typing
            };
        }
    }
}

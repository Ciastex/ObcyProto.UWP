using System.Collections.Generic;
using System.Linq;

namespace ObcyProto.UWP.SockJS
{
    internal static class PacketDecoderExtensions
    {
        public static Packet GetFirstPacket(this List<Packet> packets)
        {
            return packets.FirstOrDefault();
        }
    }
}

using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class PongPacket : Packet
    {
        public PongPacket()
        {
            Header = "_gdzie";
        }
    }
}

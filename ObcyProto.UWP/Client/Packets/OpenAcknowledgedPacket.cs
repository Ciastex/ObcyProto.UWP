using ObcyProto.UWP.SockJS;

namespace ObcyProto.UWP.Client.Packets
{
    public sealed class OpenAcknowledgedPacket : Packet
    {
        public OpenAcknowledgedPacket()
        {
            Header = "_owack";
        }
    }
}

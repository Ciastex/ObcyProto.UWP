namespace ObcyProto.UWP.SockJS
{
    internal enum PacketType
    {
        ConnectionOpen,
        ConnectionClose,
        SocketHeartbeat,
        SocketMessage,
        BinaryData,
        Invalid
    }
}

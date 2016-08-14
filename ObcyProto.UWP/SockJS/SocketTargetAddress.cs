namespace ObcyProto.UWP.SockJS
{
    internal class SocketTargetAddress
    {
        public int Port { get; }

        public string SocketNumber { get; }
        public string SocketSeed { get; }

        public string Origin => "http://6obcy.in";
        public string ServerAddress => "server.6obcy.pl";

        public SocketTargetAddress()
        {
            Port = SocketAddressGenerator.GeneratePortNumber();
            SocketNumber = SocketAddressGenerator.GenerateRandomSocketNumber();
            SocketSeed = SocketAddressGenerator.GenerateRandomSocketSeed(8);
        }

        public override string ToString()
        {
            return $"ws://{ServerAddress}:{Port}/echoup/{SocketNumber}/{SocketSeed}/websocket";
        }

        public static implicit operator string(SocketTargetAddress sta)
        {
            return sta.ToString();
        }
    }
}

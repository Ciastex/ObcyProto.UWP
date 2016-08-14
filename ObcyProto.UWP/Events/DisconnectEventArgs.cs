namespace ObcyProto.UWP.Events
{
    public class DisconnectEventArgs
    {
        public ushort Code { get; }
        public string Reason { get; }

        public DisconnectEventArgs(ushort code, string reason)
        {
            Code = code;
            Reason = reason;
        }
    }
}

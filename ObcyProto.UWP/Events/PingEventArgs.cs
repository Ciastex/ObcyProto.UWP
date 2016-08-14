using System;

namespace ObcyProto.UWP.Events
{
    public class PingEventArgs : EventArgs
    {
        public DateTime ReceivedDate { get; }
        public static int PingCount { get; set; }

        public PingEventArgs(DateTime receivedDate)
        {
            ReceivedDate = receivedDate;
        }
    }
}

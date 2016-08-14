using System;

namespace ObcyProto.UWP.Events
{
    public class HeartbeatEventArgs : EventArgs
    {
        public DateTime ReceivedDate { get; }

        public HeartbeatEventArgs(DateTime receivedDate)
        {
            ReceivedDate = receivedDate;
        }
    }
}

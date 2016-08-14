using System;
using ObcyProto.UWP.Client;

namespace ObcyProto.UWP.Events
{
    public class StrangerFoundEventArgs : EventArgs
    {
        public StrangerInfo StrangerInfo { get; }

        public StrangerFoundEventArgs(StrangerInfo strangerInfo)
        {
            StrangerInfo = strangerInfo;
        }
    }
}

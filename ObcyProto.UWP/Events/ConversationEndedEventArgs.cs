using System;
using ObcyProto.UWP.Client;

namespace ObcyProto.UWP.Events
{
    public class ConversationEndedEventArgs : EventArgs
    {
        public DisconnectInfo DisconnectInfo { get; }

        public ConversationEndedEventArgs(DisconnectInfo disconnectInfo)
        {
            DisconnectInfo = disconnectInfo;
        }
    }
}

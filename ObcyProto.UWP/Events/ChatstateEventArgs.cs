using System;
using ObcyProto.UWP.Client;

namespace ObcyProto.UWP.Events
{
    public class ChatstateEventArgs : EventArgs
    {
        public ChatState ChatState { get; }

        public ChatstateEventArgs(ChatState chatState)
        {
            ChatState = chatState;
        }
    }
}

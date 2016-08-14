using System;
using ObcyProto.UWP.Client;

namespace ObcyProto.UWP.Events
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; }

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}

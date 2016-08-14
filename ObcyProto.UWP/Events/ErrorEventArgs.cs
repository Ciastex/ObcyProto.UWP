using System;

namespace ObcyProto.UWP.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}

using System;

namespace ObcyProto.UWP.Events
{
    public class JsonEventArgs : EventArgs
    {
        public string JsonString { get; }

        public JsonEventArgs(string jsonString)
        {
            JsonString = jsonString;
        }
    }
}

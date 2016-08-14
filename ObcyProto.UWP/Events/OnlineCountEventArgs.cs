﻿using System;

namespace ObcyProto.UWP.Events
{
    public class OnlineCountEventArgs : EventArgs
    {
        public int CurrentCount { get; }

        public OnlineCountEventArgs(int currentCount)
        {
            CurrentCount = currentCount;
        }
    }
}

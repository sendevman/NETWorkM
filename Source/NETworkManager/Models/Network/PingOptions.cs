﻿namespace NETworkManager.Models.Network
{
    public class PingOptions
    {
        public int Attempts { get; set; }
        public int WaitTime { get; set; }
        public int Timeout { get; set; }
        public byte[] Buffer { get; set; }
        public int TTL { get; set; }
        public bool DontFragment { get; set; }
        public int ExceptionCancelCount { get; set; }

        public PingOptions()
        {

        }

        public PingOptions(int attempts, int waitTime, int timeout, byte[] buffer, int ttl, bool dontFragment, int exceptionCancelCount)
        {
            Attempts = attempts;
            WaitTime = waitTime;
            Timeout = timeout;
            Buffer = buffer;
            TTL = ttl;
            DontFragment = dontFragment;
            ExceptionCancelCount = exceptionCancelCount;
        }
    }
}
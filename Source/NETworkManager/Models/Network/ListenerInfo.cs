﻿using NETworkManager.Utilities;
using System.Net;
using static NETworkManager.Models.Network.Listener;

namespace NETworkManager.Models.Network
{
    public class ListenerInfo
    {
        public Protocol Protocol { get; set; }
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }

        public int IPAddressInt32 => IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? IPv4AddressHelper.ConvertToInt32(IPAddress) : 0;

        public ListenerInfo()
        {

        }

        public ListenerInfo(Protocol protocol, IPAddress ipddress, int port)
        {
            Protocol = protocol;
            IPAddress = ipddress;
            Port = port;
        }
    }
}
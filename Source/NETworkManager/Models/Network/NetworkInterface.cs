﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NETworkManager.Models.Network
{
    public class NetworkInterface
    {
        #region Events
        //public event EventHandler<ProgressChangedArgs> ConfigureProgressChanged;

        //protected virtual void OnConfigureProgressChanged(ProgressChangedArgs e)
        //{
        //    ConfigureProgressChanged?.Invoke(this, e);
        //}
        #endregion

        #region Methods
        public static Task<List<NetworkInterfaceInfo>> GetNetworkInterfacesAsync()
        {
            return Task.Run(() => GetNetworkInterfaces());
        }

        public static List<NetworkInterfaceInfo> GetNetworkInterfaces()
        {
            List<NetworkInterfaceInfo> listNetworkInterfaceInfo = new List<NetworkInterfaceInfo>();

            foreach (System.Net.NetworkInformation.NetworkInterface networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
                    continue;

                List<IPAddress> listIPv4Address = new List<IPAddress>();
                List<IPAddress> listSubnetmask = new List<IPAddress>();
                List<IPAddress> listIPv6AddressLinkLocal = new List<IPAddress>();
                List<IPAddress> listIPv6Address = new List<IPAddress>();

                DateTime dhcpLeaseObtained = new DateTime();
                DateTime dhcpLeaseExpires = new DateTime();

                foreach (UnicastIPAddressInformation unicastIPAddrInfo in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        listIPv4Address.Add(unicastIPAddrInfo.Address);
                        listSubnetmask.Add(unicastIPAddrInfo.IPv4Mask);

                        dhcpLeaseExpires = (DateTime.UtcNow + TimeSpan.FromSeconds(unicastIPAddrInfo.AddressPreferredLifetime)).ToLocalTime();
                        dhcpLeaseObtained = (DateTime.UtcNow + TimeSpan.FromSeconds(unicastIPAddrInfo.AddressValidLifetime) - TimeSpan.FromSeconds(unicastIPAddrInfo.DhcpLeaseLifetime)).ToLocalTime();
                    }
                    else if (unicastIPAddrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        if (unicastIPAddrInfo.Address.IsIPv6LinkLocal)
                            listIPv6AddressLinkLocal.Add(unicastIPAddrInfo.Address);
                        else
                            listIPv6Address.Add(unicastIPAddrInfo.Address);
                    }
                }

                List<IPAddress> listIPv4Gateway = new List<IPAddress>();
                List<IPAddress> listIPv6Gateway = new List<IPAddress>();

                foreach (GatewayIPAddressInformation gatewayIPAddrInfo in networkInterface.GetIPProperties().GatewayAddresses)
                {
                    if (gatewayIPAddrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        listIPv4Gateway.Add(gatewayIPAddrInfo.Address);
                    else if (gatewayIPAddrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        listIPv6Gateway.Add(gatewayIPAddrInfo.Address);
                }

                List<IPAddress> listDhcpServer = new List<IPAddress>();

                foreach (IPAddress dhcpServerIPAddress in networkInterface.GetIPProperties().DhcpServerAddresses)
                {
                    if (dhcpServerIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        listDhcpServer.Add(dhcpServerIPAddress);
                }

                List<IPAddress> listDnsServer = new List<IPAddress>();

                foreach (IPAddress dnsServerIPAddress in networkInterface.GetIPProperties().DnsAddresses)
                {
                    listDnsServer.Add(dnsServerIPAddress);
                }

                listNetworkInterfaceInfo.Add(new NetworkInterfaceInfo
                {
                    Id = networkInterface.Id,
                    Name = networkInterface.Name,
                    Description = networkInterface.Description,
                    Type = networkInterface.NetworkInterfaceType.ToString(),
                    PhysicalAddress = networkInterface.GetPhysicalAddress(),
                    Status = networkInterface.OperationalStatus,
                    IsOperational = networkInterface.OperationalStatus == OperationalStatus.Up ? true : false,
                    Speed = networkInterface.Speed,
                    IPv4Address = listIPv4Address.ToArray(),
                    Subnetmask = listSubnetmask.ToArray(),
                    IPv4Gateway = listIPv4Gateway.ToArray(),
                    IsDhcpEnabled = networkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled,
                    DhcpServer = listDhcpServer.ToArray(),
                    DhcpLeaseObtained = dhcpLeaseObtained,
                    DhcpLeaseExpires = dhcpLeaseExpires,
                    IPv6AddressLinkLocal = listIPv6AddressLinkLocal.ToArray(),
                    IPv6Address = listIPv6Address.ToArray(),
                    IPv6Gateway = listIPv6Gateway.ToArray(),
                    DnsSuffix = networkInterface.GetIPProperties().DnsSuffix,
                    DnsServer = listDnsServer.ToArray()
                });
            }

            return listNetworkInterfaceInfo;
        }

        public Task ConfigureNetworkInterfaceAsync(NetworkInterfaceConfig config)
        {
            return Task.Run(() => ConfigureNetworkInterface(config));
        }

        public void ConfigureNetworkInterface(NetworkInterfaceConfig config)
        {
            // IP
            string command = string.Format("netsh interface ipv4 set address name=\"{0}\" ", config.Name);
            command += config.EnableStaticIPAddress ? string.Format("source=static address={0} mask={1} gateway={2}", config.IPAddress, config.Subnetmask, config.Gateway) : "source=dhcp";

            // DNS
            command += string.Format(";netsh interface ipv4 set dnsservers name=\"{0}\" ", config.Name);
            command += config.EnableStaticDns ? string.Format("source=static address={0} register=primary validate=no", config.PrimaryDnsServer) : "source=dhcp";
            command += (config.EnableStaticDns && !string.IsNullOrEmpty(config.SecondaryDnsServer)) ? string.Format(";netsh interface ipv4 add dnsservers name=\"{0}\" address={1} index=2 validate=no", config.Name, config.SecondaryDnsServer) : "";

            // Start process with elevated rights...
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.Verb = "runas";
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.Arguments = string.Format("-NoProfile -NoLogo -Command {0}", command); ;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();

            process.WaitForExit();
        }
        #endregion
    }
}

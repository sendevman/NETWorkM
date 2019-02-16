﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using NETworkManager.ViewModels;

namespace NETworkManager.Models.Settings
{
    public static class ProfileManager
    {
        public const string ProfilesFileName = "Profiles.xml";

        public const string TagIdentifier = "tag=";

        public static ObservableCollection<ProfileInfo> Profiles { get; set; }
        public static bool ProfilesChanged { get; set; }

        public static string GetProfilesFilePath()
        {
            return Path.Combine(SettingsManager.GetSettingsLocation(), ProfilesFileName);
        }

        public static List<string> GetGroups()
        {
            var list = new List<string>();

            foreach (var profile in Profiles)
            {
                if (!list.Contains(profile.Group))
                    list.Add(profile.Group);
            }

            return list;
        }

        public static void Load(bool deserialize = true)
        {
            Profiles = new ObservableCollection<ProfileInfo>();

            if (deserialize)
                DeserializeFromFile();

            Profiles.CollectionChanged += Profiles_CollectionChanged; ;
        }

        public static void Import(bool overwrite)
        {
            if (overwrite)
                Profiles.Clear();

            DeserializeFromFile();
        }

        private static void DeserializeFromFile()
        {
            if (!File.Exists(GetProfilesFilePath()))
                return;

            var xmlSerializer = new XmlSerializer(typeof(List<ProfileInfo>));

            using (var fileStream = new FileStream(GetProfilesFilePath(), FileMode.Open))
            {
                ((List<ProfileInfo>)(xmlSerializer.Deserialize(fileStream))).ForEach(AddProfile);
            }
        }

        private static void Profiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ProfilesChanged = true;
        }

        public static void Save()
        {
            SerializeToFile();

            ProfilesChanged = false;
        }

        private static void SerializeToFile()
        {
            var xmlSerializer = new XmlSerializer(typeof(List<ProfileInfo>));

            using (var fileStream = new FileStream(GetProfilesFilePath(), FileMode.Create))
            {
                xmlSerializer.Serialize(fileStream, new List<ProfileInfo>(Profiles));
            }
        }

        public static void Reset()
        {
            if (Profiles == null)
            {
                Load(false);
                ProfilesChanged = true;
            }
            else
            {
                Profiles.Clear();
            }
        }

        public static void AddProfile(ProfileInfo profile)
        {
            Profiles.Add(profile);
        }

        public static void AddProfile(ProfileViewModel instance)
        {
            AddProfile(new ProfileInfo
            {
                Name = instance.Name?.Trim(),
                Host = instance.Host?.Trim(),
                CredentialID = instance.CredentialID,
                Group = instance.Group?.Trim(),
                Tags = instance.Tags?.Trim(),

                NetworkInterface_Enabled = instance.NetworkInterface_Enabled,
                NetworkInterface_EnableStaticIPAddress = instance.NetworkInterface_EnableStaticIPAddress,
                NetworkInterface_IPAddress = instance.NetworkInterface_IPAddress?.Trim(),
                NetworkInterface_Gateway = instance.NetworkInterface_Gateway?.Trim(),
                NetworkInterface_SubnetmaskOrCidr = instance.NetworkInterface_SubnetmaskOrCidr?.Trim(),
                NetworkInterface_EnableStaticDNS = instance.NetworkInterface_EnableStaticDNS,
                NetworkInterface_PrimaryDNSServer = instance.NetworkInterface_PrimaryDNSServer?.Trim(),
                NetworkInterface_SecondaryDNSServer = instance.NetworkInterface_SecondaryDNSServer?.Trim(),

                IPScanner_Enabled = instance.IPScanner_Enabled,
                IPScanner_InheritHost = instance.IPScanner_InheritHost,
                IPScanner_IPRange = instance.IPScanner_InheritHost ? instance.Host?.Trim() : instance.IPScanner_IPRange?.Trim(),

                PortScanner_Enabled = instance.PortScanner_Enabled,
                PortScanner_InheritHost = instance.PortScanner_InheritHost,
                PortScanner_Host = instance.PortScanner_InheritHost ? instance.Host?.Trim() : instance.PortScanner_Host?.Trim(),
                PortScanner_Ports = instance.PortScanner_Ports?.Trim(),

                Ping_Enabled = instance.Ping_Enabled,
                Ping_InheritHost = instance.Ping_InheritHost,
                Ping_Host = instance.Ping_InheritHost ? instance.Host?.Trim() : instance.Ping_Host?.Trim(),

                Traceroute_Enabled = instance.Traceroute_Enabled,
                Traceroute_InheritHost = instance.Traceroute_InheritHost,
                Traceroute_Host = instance.Traceroute_InheritHost ? instance.Host?.Trim() : instance.Traceroute_Host?.Trim(),

                DNSLookup_Enabled = instance.DNSLookup_Enabled,
                DNSLookup_InheritHost = instance.Traceroute_InheritHost,
                DNSLookup_Host = instance.DNSLookup_InheritHost ? instance.Host?.Trim() : instance.DNSLookup_Host?.Trim(),

                RemoteDesktop_Enabled = instance.RemoteDesktop_Enabled,
                RemoteDesktop_InheritHost = instance.RemoteDesktop_InheritHost,
                RemoteDesktop_Host = instance.RemoteDesktop_InheritHost ? instance.Host?.Trim() : instance.RemoteDesktop_Host?.Trim(),
                RemoteDesktop_OverrideDisplay = instance.RemoteDesktop_OverrideDisplay,
                RemoteDesktop_AdjustScreenAutomatically = instance.RemoteDesktop_AdjustScreenAutomatically,
                RemoteDesktop_UseCurrentViewSize = instance.RemoteDesktop_UseCurrentViewSize,
                RemoteDesktop_UseFixedScreenSize = instance.RemoteDesktop_UseFixedScreenSize,
                RemoteDesktop_ScreenWidth = instance.RemoteDesktop_ScreenWidth,
                RemoteDesktop_ScreenHeight = instance.RemoteDesktop_ScreenHeight,
                RemoteDesktop_UseCustomScreenSize = instance.RemoteDesktop_UseCustomScreenSize,
                RemoteDesktop_CustomScreenWidth = int.Parse(instance.RemoteDesktop_CustomScreenWidth),
                RemoteDesktop_CustomScreenHeight = int.Parse(instance.RemoteDesktop_CustomScreenHeight),
                RemoteDesktop_OverrideColorDepth = instance.RemoteDesktop_OverrideColorDepth,
                RemoteDesktop_ColorDepth = instance.RemoteDesktop_SelectedColorDepth,
                RemoteDesktop_OverridePort = instance.RemoteDesktop_OverridePort,
                RemoteDesktop_Port = instance.RemoteDesktop_Port,
                RemoteDesktop_OverrideCredSspSupport = instance.RemoteDesktop_OverrideCredSspSupport,
                RemoteDesktop_EnableCredSspSupport = instance.RemoteDesktop_EnableCredSspSupport,
                RemoteDesktop_OverrideAuthenticationLevel = instance.RemoteDesktop_OverrideAuthenticationLevel,
                RemoteDesktop_AuthenticationLevel = instance.RemoteDesktop_AuthenticationLevel,
                RemoteDesktop_OverrideApplyWindowsKeyCombinations = instance.RemoteDesktop_OverrideApplyWindowsKeyCombinations,
                RemoteDesktop_KeyboardHookMode = instance.RemoteDesktop_KeyboardHookMode.Item1,
                RemoteDesktop_OverrideRedirectClipboard = instance.RemoteDesktop_OverrideRedirectClipboard,
                RemoteDesktop_RedirectClipboard = instance.RemoteDesktop_RedirectClipboard,
                RemoteDesktop_OverrideRedirectDevices = instance.RemoteDesktop_OverrideRedirectDevices,
                RemoteDesktop_RedirectDevices = instance.RemoteDesktop_RedirectDevices,
                RemoteDesktop_OverrideRedirectDrives = instance.RemoteDesktop_OverrideRedirectDrives,
                RemoteDesktop_RedirectDrives = instance.RemoteDesktop_RedirectDrives,
                RemoteDesktop_OverrideRedirectPorts = instance.RemoteDesktop_OverrideRedirectPorts,
                RemoteDesktop_RedirectPorts = instance.RemoteDesktop_RedirectPorts,
                RemoteDesktop_OverrideRedirectSmartcards = instance.RemoteDesktop_OverrideRedirectSmartcards,
                RemoteDesktop_RedirectSmartCards = instance.RemoteDesktop_RedirectSmartCards,
                RemoteDesktop_OverrideRedirectPrinters = instance.RemoteDesktop_OverrideRedirectPrinters,
                RemoteDesktop_RedirectPrinters = instance.RemoteDesktop_RedirectPrinters,

                PowerShell_Enabled = instance.PowerShell_Enabled,
                PowerShell_EnableRemoteConsole = instance.PowerShell_EnableRemoteConsole,
                PowerShell_InheritHost = instance.PowerShell_InheritHost,
                PowerShell_Host = instance.PowerShell_InheritHost ? instance.Host?.Trim() : instance.PowerShell_Host?.Trim(),
                PowerShell_OverrideAdditionalCommandLine = instance.PowerShell_OverrideAdditionalCommandLine,
                PowerShell_AdditionalCommandLine = instance.PowerShell_AdditionalCommandLine,
                PowerShell_OverrideExecutionPolicy = instance.PowerShell_OverrideExecutionPolicy,
                PowerShell_ExecutionPolicy = instance.PowerShell_ExecutionPolicy,

                PuTTY_Enabled = instance.PuTTY_Enabled,
                PuTTY_ConnectionMode = instance.PuTTY_ConnectionMode,
                PuTTY_InheritHost = instance.PuTTY_InheritHost,
                PuTTY_HostOrSerialLine = instance.PuTTY_ConnectionMode == PuTTY.PuTTY.ConnectionMode.Serial ? instance.PuTTY_HostOrSerialLine?.Trim() : (instance.PuTTY_InheritHost ? instance.Host?.Trim() : instance.PuTTY_HostOrSerialLine?.Trim()),
                PuTTY_OverridePortOrBaud = instance.PuTTY_OverridePortOrBaud,
                PuTTY_PortOrBaud = instance.PuTTY_PortOrBaud,
                PuTTY_OverrideUsername = instance.PuTTY_OverrideUsername,
                PuTTY_Username = instance.PuTTY_Username?.Trim(),
                PuTTY_OverrideProfile = instance.PuTTY_OverrideProfile,
                PuTTY_Profile = instance.PuTTY_Profile?.Trim(),
                PuTTY_OverrideAdditionalCommandLine = instance.PuTTY_OverrideAdditionalCommandLine,
                PuTTY_AdditionalCommandLine = instance.PuTTY_AdditionalCommandLine?.Trim(),

                TigerVNC_Enabled = instance.TigerVNC_Enabled,
                TigerVNC_InheritHost = instance.TigerVNC_InheritHost,
                TigerVNC_Host = instance.TigerVNC_InheritHost ? instance.Host?.Trim() : instance.TigerVNC_Host?.Trim(),
                TigerVNC_OverridePort = instance.TigerVNC_OverridePort,
                TigerVNC_Port = instance.TigerVNC_Port,

                WakeOnLAN_Enabled = instance.WakeOnLAN_Enabled,
                WakeOnLAN_MACAddress = instance.WakeOnLAN_MACAddress?.Trim(),
                WakeOnLAN_Broadcast = instance.WakeOnLAN_Broadcast?.Trim(),
                WakeOnLAN_OverridePort = instance.WakeOnLAN_OverridePort,
                WakeOnLAN_Port = instance.WakeOnLAN_Port,

                HTTPHeaders_Enabled = instance.HTTPHeaders_Enabled,
                HTTPHeaders_Website = instance.HTTPHeaders_Website,

                Whois_Enabled = instance.Whois_Enabled,
                Whois_InheritHost = instance.Whois_InheritHost,
                Whois_Domain = instance.Whois_InheritHost ? instance.Host?.Trim() : instance.Whois_Domain?.Trim()
            });
        }

        public static void RemoveProfile(ProfileInfo profile)
        {
            Profiles.Remove(profile);
        }

        public static void RenameGroup(string oldGroup, string group)
        {
            // Go through all groups
            foreach (var profile in Profiles)
            {
                // Find specific group
                if (profile.Group != oldGroup)
                    continue;

                // Rename the group
                profile.Group = @group?.Trim();

                ProfilesChanged = true;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Heijden.DNS;
using Lextm.SharpSnmpLib.Messaging;
using NETworkManager.Models.Export;
using NETworkManager.Models.Network;
using NETworkManager.Utilities;
using static NETworkManager.Models.Network.SNMP;

namespace NETworkManager.Models.Settings
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SettingsInfo : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        [XmlIgnore] public bool SettingsChanged { get; set; }

        private string _settingsVersion = "0.0.0.0";
        public string SettingsVersion
        {
            get => _settingsVersion;
            set
            {
                if (value == _settingsVersion)
                    return;

                _settingsVersion = value;
                SettingsChanged = true;
            }
        }

        #region General 
        // General        
        private ApplicationViewManager.Name _general_DefaultApplicationViewName = GlobalStaticConfiguration.General_DefaultApplicationViewName;
        public ApplicationViewManager.Name General_DefaultApplicationViewName
        {
            get => _general_DefaultApplicationViewName;
            set
            {
                if (value == _general_DefaultApplicationViewName)
                    return;

                _general_DefaultApplicationViewName = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _general_BackgroundJobInterval = GlobalStaticConfiguration.General_BackgroundJobInterval;
        public int General_BackgroundJobInterval
        {
            get => _general_BackgroundJobInterval;
            set
            {
                if (value == _general_BackgroundJobInterval)
                    return;

                _general_BackgroundJobInterval = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _general_HistoryListEntries = GlobalStaticConfiguration.General_HistoryListEntries;
        public int General_HistoryListEntries
        {
            get => _general_HistoryListEntries;
            set
            {
                if (value == _general_HistoryListEntries)
                    return;

                _general_HistoryListEntries = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<ApplicationViewInfo> _general_ApplicationList = new ObservableCollection<ApplicationViewInfo>();
        public ObservableCollection<ApplicationViewInfo> General_ApplicationList
        {
            get => _general_ApplicationList;
            set
            {
                if (value == _general_ApplicationList)
                    return;

                _general_ApplicationList = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Window
        private bool _window_ConfirmClose;
        public bool Window_ConfirmClose
        {
            get => _window_ConfirmClose;
            set
            {
                if (value == _window_ConfirmClose)
                    return;

                _window_ConfirmClose = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _window_MinimizeInsteadOfTerminating;
        public bool Window_MinimizeInsteadOfTerminating
        {
            get => _window_MinimizeInsteadOfTerminating;
            set
            {
                if (value == _window_MinimizeInsteadOfTerminating)
                    return;

                _window_MinimizeInsteadOfTerminating = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _window_MultipleInstances;
        public bool Window_MultipleInstances
        {
            get => _window_MultipleInstances;
            set
            {
                if (value == _window_MultipleInstances)
                    return;

                _window_MultipleInstances = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _window_MinimizeToTrayInsteadOfTaskbar;
        public bool Window_MinimizeToTrayInsteadOfTaskbar
        {
            get => _window_MinimizeToTrayInsteadOfTaskbar;
            set
            {
                if (value == _window_MinimizeToTrayInsteadOfTaskbar)
                    return;

                _window_MinimizeToTrayInsteadOfTaskbar = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // TrayIcon
        private bool _trayIcon_AlwaysShowIcon;
        public bool TrayIcon_AlwaysShowIcon
        {
            get => _trayIcon_AlwaysShowIcon;
            set
            {
                if (value == _trayIcon_AlwaysShowIcon)
                    return;

                _trayIcon_AlwaysShowIcon = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Appearance
        private string _appearance_AppTheme;
        public string Appearance_AppTheme
        {
            get => _appearance_AppTheme;
            set
            {
                if (value == _appearance_AppTheme)
                    return;

                _appearance_AppTheme = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _appearance_Accent;
        public string Appearance_Accent
        {
            get => _appearance_Accent;
            set
            {
                if (value == _appearance_Accent)
                    return;

                _appearance_Accent = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _appearance_EnableTransparency;
        public bool Appearance_EnableTransparency
        {
            get => _appearance_EnableTransparency;
            set
            {
                if (value == _appearance_EnableTransparency)
                    return;

                _appearance_EnableTransparency = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _appearance_Opacity = GlobalStaticConfiguration.Appearance_Opacity;
        public double Appearance_Opacity
        {
            get => _appearance_Opacity;
            set
            {
                if (Math.Abs(value - _appearance_Opacity) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _appearance_Opacity = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Localization
        private string _localization_CultureCode;
        public string Localization_CultureCode
        {
            get => _localization_CultureCode;
            set
            {
                if (value == _localization_CultureCode)
                    return;

                _localization_CultureCode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Autostart
        private bool _autostart_StartMinimizedInTray;
        public bool Autostart_StartMinimizedInTray
        {
            get => _autostart_StartMinimizedInTray;
            set
            {
                if (value == _autostart_StartMinimizedInTray)
                    return;

                _autostart_StartMinimizedInTray = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // HotKey
        private bool _hotKey_ShowWindowEnabled;
        public bool HotKey_ShowWindowEnabled
        {
            get => _hotKey_ShowWindowEnabled;
            set
            {
                if (value == _hotKey_ShowWindowEnabled)
                    return;

                _hotKey_ShowWindowEnabled = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _hotKey_ShowWindowKey = GlobalStaticConfiguration.HotKey_ShowWindowKey;
        public int HotKey_ShowWindowKey
        {
            get => _hotKey_ShowWindowKey;
            set
            {
                if (value == _hotKey_ShowWindowKey)
                    return;

                _hotKey_ShowWindowKey = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _hotKey_ShowWindowModifier = GlobalStaticConfiguration.HotKey_ShowWindowModifier;
        public int HotKey_ShowWindowModifier
        {
            get => _hotKey_ShowWindowModifier;
            set
            {
                if (value == _hotKey_ShowWindowModifier)
                    return;

                _hotKey_ShowWindowModifier = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Update
        private bool _update_CheckForUpdatesAtStartup = true;
        public bool Update_CheckForUpdatesAtStartup
        {
            get => _update_CheckForUpdatesAtStartup;
            set
            {
                if (value == _update_CheckForUpdatesAtStartup)
                    return;

                _update_CheckForUpdatesAtStartup = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Others
        // Application view       
        private bool _expandApplicationView;
        public bool ExpandApplicationView
        {
            get => _expandApplicationView;
            set
            {
                if (value == _expandApplicationView)
                    return;

                _expandApplicationView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Network Interface       
        private string _networkInterface_SelectedInterfaceId;
        public string NetworkInterface_SelectedInterfaceId
        {
            get => _networkInterface_SelectedInterfaceId;
            set
            {
                if (value == _networkInterface_SelectedInterfaceId)
                    return;

                _networkInterface_SelectedInterfaceId = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _networkInterface_ExpandProfileView = true;
        public bool NetworkInterface_ExpandProfileView
        {
            get => _networkInterface_ExpandProfileView;
            set
            {
                if (value == _networkInterface_ExpandProfileView)
                    return;

                _networkInterface_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _networkInterface_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double NetworkInterface_ProfileWidth
        {
            get => _networkInterface_ProfileWidth;
            set
            {
                if (Math.Abs(value - _networkInterface_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _networkInterface_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region IPScanner        
        private bool _ipScanner_ShowScanResultForAllIPAddresses;
        public bool IPScanner_ShowScanResultForAllIPAddresses
        {
            get => _ipScanner_ShowScanResultForAllIPAddresses;
            set
            {
                if (value == _ipScanner_ShowScanResultForAllIPAddresses)
                    return;

                _ipScanner_ShowScanResultForAllIPAddresses = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_Threads = GlobalStaticConfiguration.IPScanner_Threads;
        public int IPScanner_Threads
        {
            get => _ipScanner_Threads;
            set
            {
                if (value == _ipScanner_Threads)
                    return;

                _ipScanner_Threads = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_ICMPAttempts = GlobalStaticConfiguration.IPScanner_ICMPAttempts;
        public int IPScanner_ICMPAttempts
        {
            get => _ipScanner_ICMPAttempts;
            set
            {
                if (value == _ipScanner_ICMPAttempts)
                    return;

                _ipScanner_ICMPAttempts = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_ICMPBuffer = GlobalStaticConfiguration.IPScanner_ICMPBuffer;
        public int IPScanner_ICMPBuffer
        {
            get => _ipScanner_ICMPBuffer;
            set
            {
                if (value == _ipScanner_ICMPBuffer)
                    return;

                _ipScanner_ICMPBuffer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _ipScanner_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> IPScanner_HostHistory
        {
            get => _ipScanner_HostHistory;
            set
            {
                if (value == _ipScanner_HostHistory)
                    return;

                _ipScanner_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_ResolveHostname = true;
        public bool IPScanner_ResolveHostname
        {
            get => _ipScanner_ResolveHostname;
            set
            {
                if (value == _ipScanner_ResolveHostname)
                    return;

                _ipScanner_ResolveHostname = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_UseCustomDNSServer;
        public bool IPScanner_UseCustomDNSServer
        {
            get => _ipScanner_UseCustomDNSServer;
            set
            {
                if (value == _ipScanner_UseCustomDNSServer)
                    return;

                _ipScanner_UseCustomDNSServer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private List<string> _ipScanner_CustomDNSServer = new List<string>();
        public List<string> IPScanner_CustomDNSServer
        {
            get => _ipScanner_CustomDNSServer;
            set
            {
                if (value == _ipScanner_CustomDNSServer)
                    return;

                _ipScanner_CustomDNSServer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_DNSPort = GlobalStaticConfiguration.IPScanner_DNSPort;
        public int IPScanner_DNSPort
        {
            get => _ipScanner_DNSPort;
            set
            {
                if (value == _ipScanner_DNSPort)
                    return;

                _ipScanner_DNSPort = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_DNSRecursion = true;
        public bool IPScanner_DNSRecursion
        {
            get => _ipScanner_DNSRecursion;
            set
            {
                if (value == _ipScanner_DNSRecursion)
                    return;

                _ipScanner_DNSRecursion = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_DNSUseResolverCache;
        public bool IPScanner_DNSUseResolverCache
        {
            get => _ipScanner_DNSUseResolverCache;
            set
            {
                if (value == _ipScanner_DNSUseResolverCache)
                    return;

                _ipScanner_DNSUseResolverCache = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private TransportType _ipScanner_DNSTransportType = GlobalStaticConfiguration.IPScanner_DNSTransportType;
        public TransportType IPScanner_DNSTransportType
        {
            get => _ipScanner_DNSTransportType;
            set
            {
                if (value == _ipScanner_DNSTransportType)
                    return;

                _ipScanner_DNSTransportType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_DNSAttempts = GlobalStaticConfiguration.IPScanner_DNSAttempts;
        public int IPScanner_DNSAttempts
        {
            get => _ipScanner_DNSAttempts;
            set
            {
                if (value == _ipScanner_DNSAttempts)
                    return;

                _ipScanner_DNSAttempts = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_DNSTimeout = GlobalStaticConfiguration.IPScanner_DNSTimeout;
        public int IPScanner_DNSTimeout
        {
            get => _ipScanner_DNSTimeout;
            set
            {
                if (value == _ipScanner_DNSTimeout)
                    return;

                _ipScanner_DNSTimeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_ResolveMACAddress;
        public bool IPScanner_ResolveMACAddress
        {
            get => _ipScanner_ResolveMACAddress;
            set
            {
                if (value == _ipScanner_ResolveMACAddress)
                    return;

                _ipScanner_ResolveMACAddress = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ipScanner_ICMPTimeout = GlobalStaticConfiguration.IPScanner_ICMPTimeout;
        public int IPScanner_ICMPTimeout
        {
            get => _ipScanner_ICMPTimeout;
            set
            {
                if (value == _ipScanner_ICMPTimeout)
                    return;

                _ipScanner_ICMPTimeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_ExpandStatistics = true;
        public bool IPScanner_ExpandStatistics
        {
            get => _ipScanner_ExpandStatistics;
            set
            {
                if (value == _ipScanner_ExpandStatistics)
                    return;

                _ipScanner_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_ExpandProfileView = true;
        public bool IPScanner_ExpandProfileView
        {
            get => _ipScanner_ExpandProfileView;
            set
            {
                if (value == _ipScanner_ExpandProfileView)
                    return;

                _ipScanner_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _ipScanner_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double IPScanner_ProfileWidth
        {
            get => _ipScanner_ProfileWidth;
            set
            {
                if (Math.Abs(value - _ipScanner_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _ipScanner_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ipScanner_ShowStatistics = true;
        public bool IPScanner_ShowStatistics
        {
            get => _ipScanner_ShowStatistics;
            set
            {
                if (value == _ipScanner_ShowStatistics)
                    return;

                _ipScanner_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _ipScanner_ExportFilePath;
        public string IPScanner_ExportFilePath
        {
            get => _ipScanner_ExportFilePath;
            set
            {
                if (value == _ipScanner_ExportFilePath)
                    return;

                _ipScanner_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _ipScanner_ExportFileType = GlobalStaticConfiguration.IPScanner_ExportFileType;
        public ExportManager.ExportFileType IPScanner_ExportFileType
        {
            get => _ipScanner_ExportFileType;
            set
            {
                if (value == _ipScanner_ExportFileType)
                    return;

                _ipScanner_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Port Scanner
        private ObservableCollection<string> _portScanner_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PortScanner_HostHistory
        {
            get => _portScanner_HostHistory;
            set
            {
                if (value == _portScanner_HostHistory)
                    return;

                _portScanner_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _portScanner_PortHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PortScanner_PortHistory
        {
            get => _portScanner_PortHistory;
            set
            {
                if (value == _portScanner_PortHistory)
                    return;

                _portScanner_PortHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _portScanner_ResolveHostname = true;
        public bool PortScanner_ResolveHostname
        {
            get => _portScanner_ResolveHostname;
            set
            {
                if (value == _portScanner_ResolveHostname)
                    return;

                _portScanner_ResolveHostname = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _portScanner_HostThreads = GlobalStaticConfiguration.PortScanner_HostThreads;
        public int PortScanner_HostThreads
        {
            get => _portScanner_HostThreads;
            set
            {
                if (value == _portScanner_HostThreads)
                    return;

                _portScanner_HostThreads = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _portScanner_PortThreads = GlobalStaticConfiguration.PortScanner_PortThreds;
        public int PortScanner_PortThreads
        {
            get => _portScanner_PortThreads;
            set
            {
                if (value == _portScanner_PortThreads)
                    return;

                _portScanner_PortThreads = value;
                SettingsChanged = true;
            }
        }

        private bool _portScanner_ShowClosed;
        public bool PortScanner_ShowClosed
        {
            get => _portScanner_ShowClosed;
            set
            {
                if (value == _portScanner_ShowClosed)
                    return;

                _portScanner_ShowClosed = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _portScanner_Timeout = GlobalStaticConfiguration.PortScanner_Timeout;
        public int PortScanner_Timeout
        {
            get => _portScanner_Timeout;
            set
            {
                if (value == _portScanner_Timeout)
                    return;

                _portScanner_Timeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _portScanner_ExpandStatistics = true;
        public bool PortScanner_ExpandStatistics
        {
            get => _portScanner_ExpandStatistics;
            set
            {
                if (value == _portScanner_ExpandStatistics)
                    return;

                _portScanner_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _portScanner_ExpandProfileView = true;
        public bool PortScanner_ExpandProfileView
        {
            get => _portScanner_ExpandProfileView;
            set
            {
                if (value == _portScanner_ExpandProfileView)
                    return;

                _portScanner_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _portScanner_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double PortScanner_ProfileWidth
        {
            get => _portScanner_ProfileWidth;
            set
            {
                if (Math.Abs(value - _portScanner_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _portScanner_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _portScanner_ShowStatistics = true;
        public bool PortScanner_ShowStatistics
        {
            get => _portScanner_ShowStatistics;
            set
            {
                if (value == _portScanner_ShowStatistics)
                    return;

                _portScanner_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _portScanner_ExportFilePath;
        public string PortScanner_ExportFilePath
        {
            get => _portScanner_ExportFilePath;
            set
            {
                if (value == _portScanner_ExportFilePath)
                    return;

                _portScanner_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _portScanner_ExportFileType = GlobalStaticConfiguration.PortScanner_ExportFileType;
        public ExportManager.ExportFileType PortScanner_ExportFileType
        {
            get => _portScanner_ExportFileType;
            set
            {
                if (value == _portScanner_ExportFileType)
                    return;

                _portScanner_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Ping
        private int _ping_Attempts;
        public int Ping_Attempts
        {
            get => _ping_Attempts;
            set
            {
                if (value == _ping_Attempts)
                    return;

                _ping_Attempts = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ping_Buffer = GlobalStaticConfiguration.Ping_Buffer;
        public int Ping_Buffer
        {
            get => _ping_Buffer;
            set
            {
                if (value == _ping_Buffer)
                    return;

                _ping_Buffer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ping_DontFragement = true;
        public bool Ping_DontFragment
        {
            get => _ping_DontFragement;
            set
            {
                if (value == _ping_DontFragement)
                    return;

                _ping_DontFragement = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _ping_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> Ping_HostHistory
        {
            get => _ping_HostHistory;
            set
            {
                if (value == _ping_HostHistory)
                    return;

                _ping_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ping_ResolveHostnamePreferIPv4 = true;
        public bool Ping_ResolveHostnamePreferIPv4
        {
            get => _ping_ResolveHostnamePreferIPv4;
            set
            {
                if (value == _ping_ResolveHostnamePreferIPv4)
                    return;

                _ping_ResolveHostnamePreferIPv4 = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ping_Timeout = GlobalStaticConfiguration.Ping_Timeout;
        public int Ping_Timeout
        {
            get => _ping_Timeout;
            set
            {
                if (value == _ping_Timeout)
                    return;

                _ping_Timeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ping_TTL = GlobalStaticConfiguration.Ping_TTL;
        public int Ping_TTL
        {
            get => _ping_TTL;
            set
            {
                if (value == _ping_TTL)
                    return;

                _ping_TTL = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ping_WaitTime = GlobalStaticConfiguration.Ping_WaitTime;
        public int Ping_WaitTime
        {
            get => _ping_WaitTime;
            set
            {
                if (value == _ping_WaitTime)
                    return;

                _ping_WaitTime = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _ping_ExceptionCancelCount = GlobalStaticConfiguration.Ping_ExceptionCancelCount;
        public int Ping_ExceptionCancelCount
        {
            get => _ping_ExceptionCancelCount;
            set
            {
                if (value == _ping_ExceptionCancelCount)
                    return;

                _ping_ExceptionCancelCount = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ping_ExpandStatistics = true;
        public bool Ping_ExpandStatistics
        {
            get => _ping_ExpandStatistics;
            set
            {
                if (value == _ping_ExpandStatistics)
                    return;

                _ping_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ping_ExpandProfileView = true;
        public bool Ping_ExpandProfileView
        {
            get => _ping_ExpandProfileView;
            set
            {
                if (value == _ping_ExpandProfileView)
                    return;

                _ping_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _ping_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double Ping_ProfileWidth
        {
            get => _ping_ProfileWidth;
            set
            {
                if (Math.Abs(value - _ping_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _ping_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ping_ShowStatistics = true;
        public bool Ping_ShowStatistics
        {
            get => _ping_ShowStatistics;
            set
            {
                if (value == _ping_ShowStatistics)
                    return;

                _ping_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _ping_ExportFilePath;
        public string Ping_ExportFilePath
        {
            get => _ping_ExportFilePath;
            set
            {
                if (value == _ping_ExportFilePath)
                    return;

                _ping_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _ping_ExportFileType = GlobalStaticConfiguration.Ping_ExportFileType;
        public ExportManager.ExportFileType Ping_ExportFileType
        {
            get => _ping_ExportFileType;
            set
            {
                if (value == _ping_ExportFileType)
                    return;

                _ping_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _ping_HighlightTimeouts = true;
        public bool Ping_HighlightTimeouts
        {
            get => _ping_HighlightTimeouts;
            set
            {
                if (value == _ping_HighlightTimeouts)
                    return;

                _ping_HighlightTimeouts = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Traceroute
        private ObservableCollection<string> _traceroute_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> Traceroute_HostHistory
        {
            get => _traceroute_HostHistory;
            set
            {
                if (value == _traceroute_HostHistory)
                    return;

                _traceroute_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _traceroute_MaximumHops = GlobalStaticConfiguration.Traceroute_MaximumHops;
        public int Traceroute_MaximumHops
        {
            get => _traceroute_MaximumHops;
            set
            {
                if (value == _traceroute_MaximumHops)
                    return;

                _traceroute_MaximumHops = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _traceroute_Timeout = GlobalStaticConfiguration.Traceroute_Timeout;
        public int Traceroute_Timeout
        {
            get => _traceroute_Timeout;
            set
            {
                if (value == _traceroute_Timeout)
                    return;

                _traceroute_Timeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _traceroute_Buffer = GlobalStaticConfiguration.Traceroute_Buffer;
        public int Traceroute_Buffer
        {
            get => _traceroute_Buffer;
            set
            {
                if (value == _traceroute_Buffer)
                    return;

                _traceroute_Buffer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _traceroute_ResolveHostname = true;
        public bool Traceroute_ResolveHostname
        {
            get => _traceroute_ResolveHostname;
            set
            {
                if (value == _traceroute_ResolveHostname)
                    return;

                _traceroute_ResolveHostname = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _traceroute_ResolveHostnamePreferIPv4 = true;
        public bool Traceroute_ResolveHostnamePreferIPv4
        {
            get => _traceroute_ResolveHostnamePreferIPv4;
            set
            {
                if (value == _traceroute_ResolveHostnamePreferIPv4)
                    return;

                _traceroute_ResolveHostnamePreferIPv4 = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _traceroute_ExpandStatistics;
        public bool Traceroute_ExpandStatistics
        {
            get => _traceroute_ExpandStatistics;
            set
            {
                if (value == _traceroute_ExpandStatistics)
                    return;

                _traceroute_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _traceroute_ExpandProfileView = true;
        public bool Traceroute_ExpandProfileView
        {
            get => _traceroute_ExpandProfileView;
            set
            {
                if (value == _traceroute_ExpandProfileView)
                    return;

                _traceroute_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _traceroute_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double Traceroute_ProfileWidth
        {
            get => _traceroute_ProfileWidth;
            set
            {
                if (Math.Abs(value - _traceroute_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _traceroute_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _traceroute_ShowStatistics = true;
        public bool Traceroute_ShowStatistics
        {
            get => _traceroute_ShowStatistics;
            set
            {
                if (value == _traceroute_ShowStatistics)
                    return;

                _traceroute_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _traceroute_ExportFilePath;
        public string Traceroute_ExportFilePath
        {
            get => _traceroute_ExportFilePath;
            set
            {
                if (value == _traceroute_ExportFilePath)
                    return;

                _traceroute_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _traceroute_ExportFileType = GlobalStaticConfiguration.Traceroute_ExportFileType;
        public ExportManager.ExportFileType Traceroute_ExportFileType
        {
            get => _traceroute_ExportFileType;
            set
            {
                if (value == _traceroute_ExportFileType)
                    return;

                _traceroute_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region DNS Lookup
        private ObservableCollection<string> _dnsLookup_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> DNSLookup_HostHistory
        {
            get => _dnsLookup_HostHistory;
            set
            {
                if (value == _dnsLookup_HostHistory)
                    return;

                _dnsLookup_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<DNSServerInfo> _dnsLookup_DNSServers = new ObservableCollection<DNSServerInfo>();
        public ObservableCollection<DNSServerInfo> DNSLookup_DNSServers
        {
            get => _dnsLookup_DNSServers;
            set
            {
                if (value == _dnsLookup_DNSServers)
                    return;

                _dnsLookup_DNSServers = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private DNSServerInfo _dnsLookup_SelectedDNSServer = new DNSServerInfo();
        public DNSServerInfo DNSLookup_SelectedDNSServer
        {
            get => _dnsLookup_SelectedDNSServer;
            set
            {
                if (value == _dnsLookup_SelectedDNSServer)
                    return;

                _dnsLookup_SelectedDNSServer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private QClass _dnsLookup_Class = GlobalStaticConfiguration.DNSLookup_Class;
        public QClass DNSLookup_Class
        {
            get => _dnsLookup_Class;
            set
            {
                if (value == _dnsLookup_Class)
                    return;

                _dnsLookup_Class = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_ShowMostCommonQueryTypes = true;
        public bool DNSLookup_ShowMostCommonQueryTypes
        {
            get => _dnsLookup_ShowMostCommonQueryTypes;
            set
            {
                if (value == _dnsLookup_ShowMostCommonQueryTypes)
                    return;

                _dnsLookup_ShowMostCommonQueryTypes = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private QType _dnsLookup_Type = GlobalStaticConfiguration.DNSLookup_Type;
        public QType DNSLookup_Type
        {
            get => _dnsLookup_Type;
            set
            {
                if (value == _dnsLookup_Type)
                    return;

                _dnsLookup_Type = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_AddDNSSuffix = true;
        public bool DNSLookup_AddDNSSuffix
        {
            get => _dnsLookup_AddDNSSuffix;
            set
            {
                if (value == _dnsLookup_AddDNSSuffix)
                    return;

                _dnsLookup_AddDNSSuffix = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_UseCustomDNSSuffix;
        public bool DNSLookup_UseCustomDNSSuffix
        {
            get => _dnsLookup_UseCustomDNSSuffix;
            set
            {
                if (value == _dnsLookup_UseCustomDNSSuffix)
                    return;

                _dnsLookup_UseCustomDNSSuffix = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _dnsLookup_CustomDNSSuffix = string.Empty;
        public string DNSLookup_CustomDNSSuffix
        {
            get => _dnsLookup_CustomDNSSuffix;
            set
            {
                if (value == _dnsLookup_CustomDNSSuffix)
                    return;

                _dnsLookup_CustomDNSSuffix = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_ResolveCNAME = true;
        public bool DNSLookup_ResolveCNAME
        {
            get => _dnsLookup_ResolveCNAME;
            set
            {
                if (value == _dnsLookup_ResolveCNAME)
                    return;

                _dnsLookup_ResolveCNAME = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_Recursion = true;
        public bool DNSLookup_Recursion
        {
            get => _dnsLookup_Recursion;
            set
            {
                if (value == _dnsLookup_Recursion)
                    return;

                _dnsLookup_Recursion = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_UseResolverCache;
        public bool DNSLookup_UseResolverCache
        {
            get => _dnsLookup_UseResolverCache;
            set
            {
                if (value == _dnsLookup_UseResolverCache)
                    return;

                _dnsLookup_UseResolverCache = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private TransportType _dnsLookup_TransportType = GlobalStaticConfiguration.DNSLookup_TransportType;
        public TransportType DNSLookup_TransportType
        {
            get => _dnsLookup_TransportType;
            set
            {
                if (value == _dnsLookup_TransportType)
                    return;

                _dnsLookup_TransportType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _dnsLookup_Attempts = GlobalStaticConfiguration.DNSLookup_Attempts;
        public int DNSLookup_Attempts
        {
            get => _dnsLookup_Attempts;
            set
            {
                if (value == _dnsLookup_Attempts)
                    return;

                _dnsLookup_Attempts = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _dnsLookup_Timeout = GlobalStaticConfiguration.DNSLookup_Timeout;
        public int DNSLookup_Timeout
        {
            get => _dnsLookup_Timeout;
            set
            {
                if (value == _dnsLookup_Timeout)
                    return;

                _dnsLookup_Timeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_ExpandStatistics = true;
        public bool DNSLookup_ExpandStatistics
        {
            get => _dnsLookup_ExpandStatistics;
            set
            {
                if (value == _dnsLookup_ExpandStatistics)
                    return;

                _dnsLookup_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_ExpandProfileView = true;
        public bool DNSLookup_ExpandProfileView
        {
            get => _dnsLookup_ExpandProfileView;
            set
            {
                if (value == _dnsLookup_ExpandProfileView)
                    return;

                _dnsLookup_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _dnsLookup_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double DNSLookup_ProfileWidth
        {
            get => _dnsLookup_ProfileWidth;
            set
            {
                if (Math.Abs(value - _dnsLookup_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _dnsLookup_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _dnsLookup_ShowStatistics = true;
        public bool DNSLookup_ShowStatistics
        {
            get => _dnsLookup_ShowStatistics;
            set
            {
                if (value == _dnsLookup_ShowStatistics)
                    return;

                _dnsLookup_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _dnsLookup_ExportFilePath;
        public string DNSLookup_ExportFilePath
        {
            get => _dnsLookup_ExportFilePath;
            set
            {
                if (value == _dnsLookup_ExportFilePath)
                    return;

                _dnsLookup_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _dnsLookup_ExportFileType = GlobalStaticConfiguration.DNSLookup_ExportFileType;
        public ExportManager.ExportFileType DNSLookup_ExportFileType
        {
            get => _dnsLookup_ExportFileType;
            set
            {
                if (value == _dnsLookup_ExportFileType)
                    return;

                _dnsLookup_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region RemoteDesktop 
        private ObservableCollection<string> _remoteDesktop_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> RemoteDesktop_HostHistory
        {
            get => _remoteDesktop_HostHistory;
            set
            {
                if (value == _remoteDesktop_HostHistory)
                    return;

                _remoteDesktop_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_AdjustScreenAutomatically;
        public bool RemoteDesktop_AdjustScreenAutomatically
        {
            get => _remoteDesktop_AdjustScreenAutomatically;
            set
            {
                if (value == _remoteDesktop_AdjustScreenAutomatically)
                    return;

                _remoteDesktop_AdjustScreenAutomatically = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_UseCurrentViewSize;
        public bool RemoteDesktop_UseCurrentViewSize
        {
            get => _remoteDesktop_UseCurrentViewSize;
            set
            {
                if (value == _remoteDesktop_UseCurrentViewSize)
                    return;

                _remoteDesktop_UseCurrentViewSize = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_UseFixedScreenSize = true;
        public bool RemoteDesktop_UseFixedScreenSize
        {
            get => _remoteDesktop_UseFixedScreenSize;
            set
            {
                if (value == _remoteDesktop_UseFixedScreenSize)
                    return;

                _remoteDesktop_UseFixedScreenSize = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_ScreenWidth = GlobalStaticConfiguration.RemoteDesktop_ScreenWidth;
        public int RemoteDesktop_ScreenWidth
        {
            get => _remoteDesktop_ScreenWidth;
            set
            {
                if (value == _remoteDesktop_ScreenWidth)
                    return;

                _remoteDesktop_ScreenWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_ScreenHeight = GlobalStaticConfiguration.RemoteDesktop_ScreenHeight;
        public int RemoteDesktop_ScreenHeight
        {
            get => _remoteDesktop_ScreenHeight;
            set
            {
                if (value == _remoteDesktop_ScreenHeight)
                    return;

                _remoteDesktop_ScreenHeight = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_UseCustomScreenSize;
        public bool RemoteDesktop_UseCustomScreenSize
        {
            get => _remoteDesktop_UseCustomScreenSize;
            set
            {
                if (value == _remoteDesktop_UseCustomScreenSize)
                    return;

                _remoteDesktop_UseCustomScreenSize = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_CustomScreenWidth;
        public int RemoteDesktop_CustomScreenWidth
        {
            get => _remoteDesktop_CustomScreenWidth;
            set
            {
                if (value == _remoteDesktop_CustomScreenWidth)
                    return;

                _remoteDesktop_CustomScreenWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_CustomScreenHeight;
        public int RemoteDesktop_CustomScreenHeight
        {
            get => _remoteDesktop_CustomScreenHeight;
            set
            {
                if (value == _remoteDesktop_CustomScreenHeight)
                    return;

                _remoteDesktop_CustomScreenHeight = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_ColorDepth = GlobalStaticConfiguration.RemoteDesktop_ColorDepth;
        public int RemoteDesktop_ColorDepth
        {
            get => _remoteDesktop_ColorDepth;
            set
            {
                if (value == _remoteDesktop_ColorDepth)
                    return;

                _remoteDesktop_ColorDepth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_Port = GlobalStaticConfiguration.RemoteDesktop_Port;
        public int RemoteDesktop_Port
        {
            get => _remoteDesktop_Port;
            set
            {
                if (value == _remoteDesktop_Port)
                    return;

                _remoteDesktop_Port = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_EnableCredSspSupport = true;
        public bool RemoteDesktop_EnableCredSspSupport
        {
            get => _remoteDesktop_EnableCredSspSupport;
            set
            {
                if (value == _remoteDesktop_EnableCredSspSupport)
                    return;

                _remoteDesktop_EnableCredSspSupport = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private uint _remoteDesktop_AuthenticationLevel = GlobalStaticConfiguration.RemoteDesktop_AuthenticationLevel;
        public uint RemoteDesktop_AuthenticationLevel
        {
            get => _remoteDesktop_AuthenticationLevel;
            set
            {
                if (value == _remoteDesktop_AuthenticationLevel)
                    return;

                _remoteDesktop_AuthenticationLevel = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _remoteDesktop_KeyboardHookMode = GlobalStaticConfiguration.RemoteDesktop_KeyboardHookMode;
        public int RemoteDesktop_KeyboardHookMode
        {
            get => _remoteDesktop_KeyboardHookMode;
            set
            {
                if (value == _remoteDesktop_KeyboardHookMode)
                    return;

                _remoteDesktop_KeyboardHookMode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_RedirectClipboard = true;
        public bool RemoteDesktop_RedirectClipboard
        {
            get => _remoteDesktop_RedirectClipboard;
            set
            {
                if (value == _remoteDesktop_RedirectClipboard)
                    return;

                _remoteDesktop_RedirectClipboard = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_RedirectDevices;
        public bool RemoteDesktop_RedirectDevices
        {
            get => _remoteDesktop_RedirectDevices;
            set
            {
                if (value == _remoteDesktop_RedirectDevices)
                    return;

                _remoteDesktop_RedirectDevices = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_RedirectDrives;
        public bool RemoteDesktop_RedirectDrives
        {
            get => _remoteDesktop_RedirectDrives;
            set
            {
                if (value == _remoteDesktop_RedirectDrives)
                    return;

                _remoteDesktop_RedirectDrives = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_RedirectPorts;
        public bool RemoteDesktop_RedirectPorts
        {
            get => _remoteDesktop_RedirectPorts;
            set
            {
                if (value == _remoteDesktop_RedirectPorts)
                    return;

                _remoteDesktop_RedirectPorts = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_RedirectSmartCards;
        public bool RemoteDesktop_RedirectSmartCards
        {
            get => _remoteDesktop_RedirectSmartCards;
            set
            {
                if (value == _remoteDesktop_RedirectSmartCards)
                    return;

                _remoteDesktop_RedirectSmartCards = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_RedirectPrinters;
        public bool RemoteDesktop_RedirectPrinters
        {
            get => _remoteDesktop_RedirectPrinters;
            set
            {
                if (value == _remoteDesktop_RedirectPrinters)
                    return;

                _remoteDesktop_RedirectPrinters = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _remoteDesktop_ExpandProfileView = true;
        public bool RemoteDesktop_ExpandProfileView
        {
            get => _remoteDesktop_ExpandProfileView;
            set
            {
                if (value == _remoteDesktop_ExpandProfileView)
                    return;

                _remoteDesktop_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _remoteDesktop_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double RemoteDesktop_ProfileWidth
        {
            get => _remoteDesktop_ProfileWidth;
            set
            {
                if (Math.Abs(value - _remoteDesktop_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _remoteDesktop_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region PowerShell
        private ObservableCollection<string> _powerShell_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PowerShell_HostHistory
        {
            get => _powerShell_HostHistory;
            set
            {
                if (value == _powerShell_HostHistory)
                    return;

                _powerShell_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _powerShell_ApplicationFilePath = GlobalStaticConfiguration.PowerShell_ApplicationFileLocationPowerShell;
        public string PowerShell_ApplicationFilePath
        {
            get => _powerShell_ApplicationFilePath;
            set
            {
                if (value == _powerShell_ApplicationFilePath)
                    return;

                _powerShell_ApplicationFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _powerShell_AdditionalCommandLine;
        public string PowerShell_AdditionalCommandLine
        {
            get => _powerShell_AdditionalCommandLine;
            set
            {
                if (value == _powerShell_AdditionalCommandLine)
                    return;

                _powerShell_AdditionalCommandLine = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private PowerShell.PowerShell.ExecutionPolicy _powerShell_ExecutionPolicy = GlobalStaticConfiguration.PowerShell_ExecutionPolicy;
        public PowerShell.PowerShell.ExecutionPolicy PowerShell_ExecutionPolicy
        {
            get => _powerShell_ExecutionPolicy;
            set
            {
                if (value == _powerShell_ExecutionPolicy)
                    return;

                _powerShell_ExecutionPolicy = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _powerShell_ExpandProfileView = true;
        public bool PowerShell_ExpandProfileView
        {
            get => _powerShell_ExpandProfileView;
            set
            {
                if (value == _powerShell_ExpandProfileView)
                    return;

                _powerShell_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _powerShell_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double PowerShell_ProfileWidth
        {
            get => _powerShell_ProfileWidth;
            set
            {
                if (Math.Abs(value - _powerShell_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _powerShell_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region PuTTY
        private ObservableCollection<string> _puTTY_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PuTTY_HostHistory
        {
            get => _puTTY_HostHistory;
            set
            {
                if (value == _puTTY_HostHistory)
                    return;

                _puTTY_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private PuTTY.PuTTY.ConnectionMode _puTTY_DefaultConnectionMode = GlobalStaticConfiguration.PuTTY_DefaultConnectionMode;
        public PuTTY.PuTTY.ConnectionMode PuTTY_DefaultConnectionMode
        {
            get => _puTTY_DefaultConnectionMode;
            set
            {
                if (value == _puTTY_DefaultConnectionMode)
                    return;

                _puTTY_DefaultConnectionMode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _puTTY_Username;
        public string PuTTY_Username
        {
            get => _puTTY_Username;
            set
            {
                if (value == _puTTY_Username)
                    return;

                _puTTY_Username = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _puTTY_Profile;
        public string PuTTY_Profile
        {
            get => _puTTY_Profile;
            set
            {
                if (value == _puTTY_Profile)
                    return;

                _puTTY_Profile = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _puTTY_AdditionalCommandLine;
        public string PuTTY_AdditionalCommandLine
        {
            get => _puTTY_AdditionalCommandLine;
            set
            {
                if (value == _puTTY_AdditionalCommandLine)
                    return;

                _puTTY_AdditionalCommandLine = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _puTTY_SerialLineHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PuTTY_SerialLineHistory
        {
            get => _puTTY_SerialLineHistory;
            set
            {
                if (value == _puTTY_SerialLineHistory)
                    return;

                _puTTY_SerialLineHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _puTTY_PortHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PuTTY_PortHistory
        {
            get => _puTTY_PortHistory;
            set
            {
                if (value == _puTTY_PortHistory)
                    return;

                _puTTY_PortHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _puTTY_BaudHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PuTTY_BaudHistory
        {
            get => _puTTY_BaudHistory;
            set
            {
                if (value == _puTTY_BaudHistory)
                    return;

                _puTTY_BaudHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _puTTY_UsernameHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PuTTY_UsernameHistory
        {
            get => _puTTY_UsernameHistory;
            set
            {
                if (value == _puTTY_UsernameHistory)
                    return;

                _puTTY_UsernameHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _puTTY_ProfileHistory = new ObservableCollection<string>();
        public ObservableCollection<string> PuTTY_ProfileHistory
        {
            get => _puTTY_ProfileHistory;
            set
            {
                if (value == _puTTY_ProfileHistory)
                    return;

                _puTTY_ProfileHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _puTTY_ExpandProfileView = true;
        public bool PuTTY_ExpandProfileView
        {
            get => _puTTY_ExpandProfileView;
            set
            {
                if (value == _puTTY_ExpandProfileView)
                    return;

                _puTTY_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _puTTY_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double PuTTY_ProfileWidth
        {
            get => _puTTY_ProfileWidth;
            set
            {
                if (Math.Abs(value - _puTTY_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _puTTY_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _puTTY_ApplicationFilePath;
        public string PuTTY_ApplicationFilePath
        {
            get => _puTTY_ApplicationFilePath;
            set
            {
                if (value == _puTTY_ApplicationFilePath)
                    return;

                _puTTY_ApplicationFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _puTTY_SerialLine = GlobalStaticConfiguration.PuTTY_SerialLine;
        public string PuTTY_SerialLine
        {
            get => _puTTY_SerialLine;
            set
            {
                if (value == _puTTY_SerialLine)
                    return;

                _puTTY_SerialLine = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _puTTY_SSHPort = GlobalStaticConfiguration.PuTTY_SSHPort;
        public int PuTTY_SSHPort
        {
            get => _puTTY_SSHPort;
            set
            {
                if (value == _puTTY_SSHPort)
                    return;

                _puTTY_SSHPort = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _puTTY_TelnetPort = GlobalStaticConfiguration.PuTTY_TelnetPort;
        public int PuTTY_TelnetPort
        {
            get => _puTTY_TelnetPort;
            set
            {
                if (value == _puTTY_TelnetPort)
                    return;

                _puTTY_TelnetPort = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _puTTY_RloginPort = GlobalStaticConfiguration.PuTTY_RloginPort;
        public int PuTTY_RloginPort
        {
            get => _puTTY_RloginPort;
            set
            {
                if (value == _puTTY_RloginPort)
                    return;

                _puTTY_RloginPort = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _puTTY_BaudRate = GlobalStaticConfiguration.PuTTY_BaudRate;
        public int PuTTY_BaudRate
        {
            get => _puTTY_BaudRate;
            set
            {
                if (value == _puTTY_BaudRate)
                    return;

                _puTTY_BaudRate = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _puTTY_DefaultRaw = GlobalStaticConfiguration.PuTTY_Raw;
        public int PuTTY_DefaultRaw
        {
            get => _puTTY_DefaultRaw;
            set
            {
                if (value == _puTTY_DefaultRaw)
                    return;

                _puTTY_DefaultRaw = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region TigerVNC
        private ObservableCollection<string> _tigerVNC_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> TigerVNC_HostHistory
        {
            get => _tigerVNC_HostHistory;
            set
            {
                if (value == _tigerVNC_HostHistory)
                    return;

                _tigerVNC_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<int> _tigerVNC_PortHistory = new ObservableCollection<int>();
        public ObservableCollection<int> TigerVNC_PortHistory
        {
            get => _tigerVNC_PortHistory;
            set
            {
                if (value == _tigerVNC_PortHistory)
                    return;

                _tigerVNC_PortHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _tigerVNC_ExpandProfileView = true;
        public bool TigerVNC_ExpandProfileView
        {
            get => _tigerVNC_ExpandProfileView;
            set
            {
                if (value == _tigerVNC_ExpandProfileView)
                    return;

                _tigerVNC_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _tigerVNC_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double TigerVNC_ProfileWidth
        {
            get => _tigerVNC_ProfileWidth;
            set
            {
                if (Math.Abs(value - _tigerVNC_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _tigerVNC_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _tigerVNC_ApplicationFilePath;
        public string TigerVNC_ApplicationFilePath
        {
            get => _tigerVNC_ApplicationFilePath;
            set
            {
                if (value == _tigerVNC_ApplicationFilePath)
                    return;

                _tigerVNC_ApplicationFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _tigerVNC_Port = GlobalStaticConfiguration.TigerVNC_DefaultVNCPort;
        public int TigerVNC_Port
        {
            get => _tigerVNC_Port;
            set
            {
                if (value == _tigerVNC_Port)
                    return;

                _tigerVNC_Port = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region SNMP
        private WalkMode _snmp_WalkMode = GlobalStaticConfiguration.SNMP_WalkMode;
        public WalkMode SNMP_WalkMode
        {
            get => _snmp_WalkMode;
            set
            {
                if (value == _snmp_WalkMode)
                    return;

                _snmp_WalkMode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _snmp_Timeout = GlobalStaticConfiguration.SNMP_Timeout;
        public int SNMP_Timeout
        {
            get => _snmp_Timeout;
            set
            {
                if (value == _snmp_Timeout)
                    return;

                _snmp_Timeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _snmp_port = 161;
        public int SNMP_Port
        {
            get => _snmp_port;
            set
            {
                if (value == _snmp_port)
                    return;

                _snmp_port = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _snmp_ResolveHostnamePreferIPv4 = true;
        public bool SNMP_ResolveHostnamePreferIPv4
        {
            get => _snmp_ResolveHostnamePreferIPv4;
            set
            {
                if (value == _snmp_ResolveHostnamePreferIPv4)
                    return;

                _snmp_ResolveHostnamePreferIPv4 = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _snmp_HostHistory = new ObservableCollection<string>();
        public ObservableCollection<string> SNMP_HostHistory
        {
            get => _snmp_HostHistory;
            set
            {
                if (value == _snmp_HostHistory)
                    return;

                _snmp_HostHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _snmp_OIDHistory = new ObservableCollection<string>();
        public ObservableCollection<string> SNMP_OIDHistory
        {
            get => _snmp_OIDHistory;
            set
            {
                if (value == _snmp_OIDHistory)
                    return;

                _snmp_OIDHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private SNMPMode _snmp_Mode = GlobalStaticConfiguration.SNMP_Mode;
        public SNMPMode SNMP_Mode
        {
            get => _snmp_Mode;
            set
            {
                if (value == _snmp_Mode)
                    return;

                _snmp_Mode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private SNMPVersion _snmp_Version = GlobalStaticConfiguration.SNMP_Version;
        public SNMPVersion SNMP_Version
        {
            get => _snmp_Version;
            set
            {
                if (value == _snmp_Version)
                    return;

                _snmp_Version = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _snmp_ExpandStatistics = true;
        public bool SNMP_ExpandStatistics
        {
            get => _snmp_ExpandStatistics;
            set
            {
                if (value == _snmp_ExpandStatistics)
                    return;

                _snmp_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private SNMPV3Security _snmp_Security = GlobalStaticConfiguration.SNMP_Security;
        public SNMPV3Security SNMP_Security
        {
            get => _snmp_Security;
            set
            {
                if (value == _snmp_Security)
                    return;

                _snmp_Security = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private SNMPV3AuthenticationProvider _snmp_AuthenticationProvider = GlobalStaticConfiguration.SNMP_AuthenticationProvider;
        public SNMPV3AuthenticationProvider SNMP_AuthenticationProvider
        {
            get => _snmp_AuthenticationProvider;
            set
            {
                if (value == _snmp_AuthenticationProvider)
                    return;

                _snmp_AuthenticationProvider = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private SNMPV3PrivacyProvider _snmp_PrivacyProvider = GlobalStaticConfiguration.SNMP_PrivacyProvider;
        public SNMPV3PrivacyProvider SNMP_PrivacyProvider
        {
            get => _snmp_PrivacyProvider;
            set
            {
                if (value == _snmp_PrivacyProvider)
                    return;

                _snmp_PrivacyProvider = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _snmp_ShowStatistics = true;
        public bool SNMP_ShowStatistics
        {
            get => _snmp_ShowStatistics;
            set
            {
                if (value == _snmp_ShowStatistics)
                    return;

                _snmp_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _snmp_ExportFilePath;
        public string SNMP_ExportFilePath
        {
            get => _snmp_ExportFilePath;
            set
            {
                if (value == _snmp_ExportFilePath)
                    return;

                _snmp_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _snmp_ExportFileType = GlobalStaticConfiguration.SNMP_ExportFileType;
        public ExportManager.ExportFileType SNMP_ExportFileType
        {
            get => _snmp_ExportFileType;
            set
            {
                if (value == _snmp_ExportFileType)
                    return;

                _snmp_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region WakeOnLAN
        private int _wakeOnLAN_Port = GlobalStaticConfiguration.WakeOnLAN_Port;
        public int WakeOnLAN_Port
        {
            get => _wakeOnLAN_Port;
            set
            {
                if (value == _wakeOnLAN_Port)
                    return;

                _wakeOnLAN_Port = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _wakeOnLAN_ExpandClientView = true;
        public bool WakeOnLAN_ExpandClientView
        {
            get => _wakeOnLAN_ExpandClientView;
            set
            {
                if (value == _wakeOnLAN_ExpandClientView)
                    return;

                _wakeOnLAN_ExpandClientView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _wakeOnLAN_ClientWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double WakeOnLAN_ClientWidth
        {
            get => _wakeOnLAN_ClientWidth;
            set
            {
                if (Math.Abs(value - _wakeOnLAN_ClientWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _wakeOnLAN_ClientWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region HTTP Headers
        private ObservableCollection<string> _httpHeaders_WebsiteUriHistory = new ObservableCollection<string>();
        public ObservableCollection<string> HTTPHeaders_WebsiteUriHistory
        {
            get => _httpHeaders_WebsiteUriHistory;
            set
            {
                if (value == _httpHeaders_WebsiteUriHistory)
                    return;

                _httpHeaders_WebsiteUriHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _httpHeaders_Timeout = GlobalStaticConfiguration.HTTPHeaders_Timeout;
        public int HTTPHeaders_Timeout
        {
            get => _httpHeaders_Timeout;
            set
            {
                if (value == _httpHeaders_Timeout)
                    return;

                _httpHeaders_Timeout = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _httpHeaders_ExpandStatistics = true;
        public bool HTTPHeaders_ExpandStatistics
        {
            get => _httpHeaders_ExpandStatistics;
            set
            {
                if (value == _httpHeaders_ExpandStatistics)
                    return;

                _httpHeaders_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _httpHeaders_ExpandProfileView = true;
        public bool HTTPHeaders_ExpandProfileView
        {
            get => _httpHeaders_ExpandProfileView;
            set
            {
                if (value == _httpHeaders_ExpandProfileView)
                    return;

                _httpHeaders_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _httpHeaders_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double HTTPHeaders_ProfileWidth
        {
            get => _httpHeaders_ProfileWidth;
            set
            {
                if (Math.Abs(value - _httpHeaders_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _httpHeaders_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _httpHeaders_ShowStatistics = true;
        public bool HTTPHeaders_ShowStatistics
        {
            get => _httpHeaders_ShowStatistics;
            set
            {
                if (value == _httpHeaders_ShowStatistics)
                    return;

                _httpHeaders_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _httpHeaders_ExportFilePath;
        public string HTTPHeaders_ExportFilePath
        {
            get => _httpHeaders_ExportFilePath;
            set
            {
                if (value == _httpHeaders_ExportFilePath)
                    return;

                _httpHeaders_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _httpHeaders_ExportFileType = GlobalStaticConfiguration.HTTPHeaders_ExportFileType;
        public ExportManager.ExportFileType HTTPHeaders_ExportFileType
        {
            get => _httpHeaders_ExportFileType;
            set
            {
                if (value == _httpHeaders_ExportFileType)
                    return;

                _httpHeaders_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Subnet Calculator

        #region Calculator
        private ObservableCollection<string> _subnetCalculator_Calculator_SubnetHistory = new ObservableCollection<string>();
        public ObservableCollection<string> SubnetCalculator_Calculator_SubnetHistory
        {
            get => _subnetCalculator_Calculator_SubnetHistory;
            set
            {
                if (value == _subnetCalculator_Calculator_SubnetHistory)
                    return;

                _subnetCalculator_Calculator_SubnetHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Subnetting
        private ObservableCollection<string> _subnetCalculator_Subnetting_SubnetHistory = new ObservableCollection<string>();
        public ObservableCollection<string> SubnetCalculator_Subnetting_SubnetHistory
        {
            get => _subnetCalculator_Subnetting_SubnetHistory;
            set
            {
                if (value == _subnetCalculator_Subnetting_SubnetHistory)
                    return;

                _subnetCalculator_Subnetting_SubnetHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _subnetCalculator_Subnetting_NewSubnetmaskOrCIDRHistory = new ObservableCollection<string>();
        public ObservableCollection<string> SubnetCalculator_Subnetting_NewSubnetmaskOrCIDRHistory
        {
            get => _subnetCalculator_Subnetting_NewSubnetmaskOrCIDRHistory;
            set
            {
                if (value == _subnetCalculator_Subnetting_NewSubnetmaskOrCIDRHistory)
                    return;

                _subnetCalculator_Subnetting_NewSubnetmaskOrCIDRHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _subnetCalculator_Subnetting_ExportFilePath;
        public string SubnetCalculator_Subnetting_ExportFilePath
        {
            get => _subnetCalculator_Subnetting_ExportFilePath;
            set
            {
                if (value == _subnetCalculator_Subnetting_ExportFilePath)
                    return;

                _subnetCalculator_Subnetting_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _subnetCalculator_Subnetting_ExportFileType = GlobalStaticConfiguration.SubnetCalculator_Subnetting_ExportFileType;
        public ExportManager.ExportFileType SubnetCalculator_Subnetting_ExportFileType
        {
            get => _subnetCalculator_Subnetting_ExportFileType;
            set
            {
                if (value == _subnetCalculator_Subnetting_ExportFileType)
                    return;

                _subnetCalculator_Subnetting_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region WideSubnet
        private ObservableCollection<string> _subnetCalculator_WideSubnet_Subnet1 = new ObservableCollection<string>();
        public ObservableCollection<string> SubnetCalculator_WideSubnet_Subnet1
        {
            get => _subnetCalculator_WideSubnet_Subnet1;
            set
            {
                if (value == _subnetCalculator_WideSubnet_Subnet1)
                    return;

                _subnetCalculator_WideSubnet_Subnet1 = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _subnetCalculator_WideSubnet_Subnet2 = new ObservableCollection<string>();
        public ObservableCollection<string> SubnetCalculator_WideSubnet_Subnet2
        {
            get => _subnetCalculator_WideSubnet_Subnet2;
            set
            {
                if (value == _subnetCalculator_WideSubnet_Subnet2)
                    return;

                _subnetCalculator_WideSubnet_Subnet2 = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #endregion

        #region Lookup
        private ObservableCollection<string> _lookup_OUI_MACAddressOrVendorHistory = new ObservableCollection<string>();
        public ObservableCollection<string> Lookup_OUI_MACAddressOrVendorHistory
        {
            get => _lookup_OUI_MACAddressOrVendorHistory;
            set
            {
                if (value == _lookup_OUI_MACAddressOrVendorHistory)
                    return;

                _lookup_OUI_MACAddressOrVendorHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _lookup_OUI_ExportFilePath;
        public string Lookup_OUI_ExportFilePath
        {
            get => _lookup_OUI_ExportFilePath;
            set
            {
                if (value == _lookup_OUI_ExportFilePath)
                    return;

                _lookup_OUI_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _lookup_OUI_ExportFileType = GlobalStaticConfiguration.Lookup_OUI_ExportFileType;
        public ExportManager.ExportFileType Lookup_OUI_ExportFileType
        {
            get => _lookup_OUI_ExportFileType;
            set
            {
                if (value == _lookup_OUI_ExportFileType)
                    return;

                _lookup_OUI_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<string> _lookup_Port_PortsHistory = new ObservableCollection<string>();
        public ObservableCollection<string> Lookup_Port_PortsHistory
        {
            get => _lookup_Port_PortsHistory;
            set
            {
                if (value == _lookup_Port_PortsHistory)
                    return;

                _lookup_Port_PortsHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _lookup_Port_ExportFilePath;
        public string Lookup_Port_ExportFilePath
        {
            get => _lookup_Port_ExportFilePath;
            set
            {
                if (value == _lookup_Port_ExportFilePath)
                    return;

                _lookup_Port_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _lookup_Port_ExportFileType = GlobalStaticConfiguration.Lookup_Port_ExportFileType;
        public ExportManager.ExportFileType Lookup_Port_ExportFileType
        {
            get => _lookup_Port_ExportFileType;
            set
            {
                if (value == _lookup_Port_ExportFileType)
                    return;

                _lookup_Port_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Whois
        private ObservableCollection<string> _whois_DomainHistory = new ObservableCollection<string>();
        public ObservableCollection<string> Whois_DomainHistory
        {
            get => _whois_DomainHistory;
            set
            {
                if (value == _whois_DomainHistory)
                    return;

                _whois_DomainHistory = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _whois_ExpandStatistics = true;
        public bool Whois_ExpandStatistics
        {
            get => _whois_ExpandStatistics;
            set
            {
                if (value == _whois_ExpandStatistics)
                    return;

                _whois_ExpandStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _whois_ExpandProfileView = true;
        public bool Whois_ExpandProfileView
        {
            get => _whois_ExpandProfileView;
            set
            {
                if (value == _whois_ExpandProfileView)
                    return;

                _whois_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _whois_ProfileWidth = GlobalStaticConfiguration.Profile_DefaultWidthExpanded;
        public double Whois_ProfileWidth
        {
            get => _whois_ProfileWidth;
            set
            {
                if (Math.Abs(value - _whois_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _whois_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _whois_ShowStatistics = true;
        public bool Whois_ShowStatistics
        {
            get => _whois_ShowStatistics;
            set
            {
                if (value == _whois_ShowStatistics)
                    return;

                _whois_ShowStatistics = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _whois_ExportFilePath;
        public string Whois_ExportFilePath
        {
            get => _whois_ExportFilePath;
            set
            {
                if (value == _whois_ExportFilePath)
                    return;

                _whois_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _whois_ExportFileType = GlobalStaticConfiguration.Whois_ExportFileType;
        public ExportManager.ExportFileType Whois_ExportFileType
        {
            get => _whois_ExportFileType;
            set
            {
                if (value == _whois_ExportFileType)
                    return;

                _whois_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Connections
        private bool _connections_AutoRefresh;
        public bool Connections_AutoRefresh
        {
            get => _connections_AutoRefresh;
            set
            {
                if (value == _connections_AutoRefresh)
                    return;

                _connections_AutoRefresh = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private AutoRefreshTimeInfo _connections_AutoRefreshTime = GlobalStaticConfiguration.Connections_AutoRefreshTime;
        public AutoRefreshTimeInfo Connections_AutoRefreshTime
        {
            get => _connections_AutoRefreshTime;
            set
            {
                if (value == _connections_AutoRefreshTime)
                    return;

                _connections_AutoRefreshTime = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _connections_ExportFilePath;
        public string Connections_ExportFilePath
        {
            get => _connections_ExportFilePath;
            set
            {
                if (value == _connections_ExportFilePath)
                    return;

                _connections_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _connections_ExportFileType = GlobalStaticConfiguration.Connections_ExportFileType;
        public ExportManager.ExportFileType Connections_ExportFileType
        {
            get => _connections_ExportFileType;
            set
            {
                if (value == _connections_ExportFileType)
                    return;

                _connections_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Listeners
        private bool _listeners_AutoRefresh;
        public bool Listeners_AutoRefresh
        {
            get => _listeners_AutoRefresh;
            set
            {
                if (value == _listeners_AutoRefresh)
                    return;

                _listeners_AutoRefresh = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private AutoRefreshTimeInfo _listeners_AutoRefreshTime = GlobalStaticConfiguration.Listeners_AutoRefreshTime;
        public AutoRefreshTimeInfo Listeners_AutoRefreshTime
        {
            get => _listeners_AutoRefreshTime;
            set
            {
                if (value == _listeners_AutoRefreshTime)
                    return;

                _listeners_AutoRefreshTime = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _listeners_ExportFilePath;
        public string Listeners_ExportFilePath
        {
            get => _listeners_ExportFilePath;
            set
            {
                if (value == _listeners_ExportFilePath)
                    return;

                _listeners_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _listeners_ExportFileType = GlobalStaticConfiguration.Listeners_ExportFileType;
        public ExportManager.ExportFileType Listeners_ExportFileType
        {
            get => _listeners_ExportFileType;
            set
            {
                if (value == _listeners_ExportFileType)
                    return;

                _listeners_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region ARPTable
        private bool _arpTable_AutoRefresh;
        public bool ARPTable_AutoRefresh
        {
            get => _arpTable_AutoRefresh;
            set
            {
                if (value == _arpTable_AutoRefresh)
                    return;

                _arpTable_AutoRefresh = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private AutoRefreshTimeInfo _arpTable_AutoRefreshTime = GlobalStaticConfiguration.ARPTable_AutoRefreshTime;
        public AutoRefreshTimeInfo ARPTable_AutoRefreshTime
        {
            get => _arpTable_AutoRefreshTime;
            set
            {
                if (value == _arpTable_AutoRefreshTime)
                    return;

                _arpTable_AutoRefreshTime = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _arpTable_ExportFilePath;
        public string ARPTable_ExportFilePath
        {
            get => _arpTable_ExportFilePath;
            set
            {
                if (value == _arpTable_ExportFilePath)
                    return;

                _arpTable_ExportFilePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ExportManager.ExportFileType _arpTable_ExportFileType = GlobalStaticConfiguration.ARPTable_ExportFileType;
        public ExportManager.ExportFileType ARPTable_ExportFileType
        {
            get => _arpTable_ExportFileType;
            set
            {
                if (value == _arpTable_ExportFileType)
                    return;

                _arpTable_ExportFileType = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #endregion

        #region Constructor
        public SettingsInfo()
        {
            // General
            General_ApplicationList.CollectionChanged += CollectionChanged;

            // IP Scanner
            IPScanner_HostHistory.CollectionChanged += CollectionChanged;

            // Port Scanner
            PortScanner_HostHistory.CollectionChanged += CollectionChanged;
            PortScanner_PortHistory.CollectionChanged += CollectionChanged;

            // Ping
            Ping_HostHistory.CollectionChanged += CollectionChanged;

            // Traceroute
            Traceroute_HostHistory.CollectionChanged += CollectionChanged;

            // DNS Lookup
            DNSLookup_HostHistory.CollectionChanged += CollectionChanged;
            DNSLookup_DNSServers.CollectionChanged += CollectionChanged;

            // Remote Desktop
            RemoteDesktop_HostHistory.CollectionChanged += CollectionChanged;

            // PowerShell
            PowerShell_HostHistory.CollectionChanged += CollectionChanged;

            // PuTTY
            PuTTY_HostHistory.CollectionChanged += CollectionChanged;
            PuTTY_SerialLineHistory.CollectionChanged += CollectionChanged;
            PuTTY_PortHistory.CollectionChanged += CollectionChanged;
            PuTTY_BaudHistory.CollectionChanged += CollectionChanged;
            PuTTY_UsernameHistory.CollectionChanged += CollectionChanged;
            PuTTY_ProfileHistory.CollectionChanged += CollectionChanged;

            // TigerVNC
            TigerVNC_HostHistory.CollectionChanged += CollectionChanged;
            TigerVNC_PortHistory.CollectionChanged += CollectionChanged;

            // SNMP
            SNMP_HostHistory.CollectionChanged += CollectionChanged;
            SNMP_OIDHistory.CollectionChanged += CollectionChanged;

            // HTTP Header
            HTTPHeaders_WebsiteUriHistory.CollectionChanged += CollectionChanged;

            // Subnet Calculator / Calculator
            SubnetCalculator_Calculator_SubnetHistory.CollectionChanged += CollectionChanged;

            // Subnet Calculator / Subnetting
            SubnetCalculator_Subnetting_SubnetHistory.CollectionChanged += CollectionChanged;
            SubnetCalculator_Subnetting_NewSubnetmaskOrCIDRHistory.CollectionChanged += CollectionChanged;

            // Subnet Calculator / Supernetting
            SubnetCalculator_WideSubnet_Subnet1.CollectionChanged += CollectionChanged;
            SubnetCalculator_WideSubnet_Subnet2.CollectionChanged += CollectionChanged;

            // Lookup / OUI
            Lookup_OUI_MACAddressOrVendorHistory.CollectionChanged += CollectionChanged;

            // Lookup / Port
            Lookup_Port_PortsHistory.CollectionChanged += CollectionChanged;

            // Whois
            Whois_DomainHistory.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsChanged = true;
        }
        #endregion
    }
}
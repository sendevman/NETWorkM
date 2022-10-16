﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Views;
using NETworkManager.Settings;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.Generic;
using NETworkManager.Utilities;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using NETworkManager.Controls;
using NETworkManager.Documentation;
using NETworkManager.ViewModels;
using ContextMenu = System.Windows.Controls.ContextMenu;
using NETworkManager.Profiles;
using NETworkManager.Localization;
using NETworkManager.Localization.Translators;
using NETworkManager.Update;
using NETworkManager.Models;
using NETworkManager.Models.EventSystem;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.IO;
using System.Collections.ObjectModel;
using NETworkManager.Models.Network;
using NETworkManager.Models.AWS;
using NETworkManager.Models.PowerShell;

namespace NETworkManager
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables        
        private NotifyIcon _notifyIcon;
        private StatusWindow _statusWindow;

        private readonly bool _isLoading;
        private bool _isProfileLoading;
        private bool _isProfileUpdating;
        private bool _isApplicationListLoading;

        private bool _isInTray;
        private bool _closeApplication;

        private bool _expandApplicationView;
        public bool ExpandApplicationView
        {
            get => _expandApplicationView;
            set
            {
                if (value == _expandApplicationView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.ExpandApplicationView = value;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _expandApplicationView = value;
                OnPropertyChanged();
            }
        }

        private bool _isTextBoxSearchFocused;
        public bool IsTextBoxSearchFocused
        {
            get => _isTextBoxSearchFocused;
            set
            {
                if (value == _isTextBoxSearchFocused)
                    return;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _isTextBoxSearchFocused = value;
                OnPropertyChanged();
            }
        }

        private bool _isApplicationListOpen;
        public bool IsApplicationListOpen
        {
            get => _isApplicationListOpen;
            set
            {
                if (value == _isApplicationListOpen)
                    return;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _isApplicationListOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isMouseOverApplicationList;
        public bool IsMouseOverApplicationList
        {
            get => _isMouseOverApplicationList;
            set
            {
                if (value == _isMouseOverApplicationList)
                    return;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _isMouseOverApplicationList = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _applications;
        public ICollectionView Applications
        {
            get => _applications;
            set
            {
                if (value == _applications)
                    return;

                _applications = value;
                OnPropertyChanged();
            }
        }

        private ApplicationInfo _selectedApplication;
        public ApplicationInfo SelectedApplication
        {
            get => _selectedApplication;
            set
            {
                if (_isApplicationListLoading)
                    return;

                if (Equals(value, _selectedApplication))
                    return;

                if (_selectedApplication != null)
                    OnApplicationViewHide(_selectedApplication.Name);

                if (value != null)
                    OnApplicationViewVisible(value.Name);

                _selectedApplication = value;
                OnPropertyChanged();
            }
        }

        private ApplicationName _filterLastViewName;
        private int? _filterLastCount;

        private string _search = string.Empty;
        public string Search
        {
            get => _search;
            set
            {
                if (value == _search)
                    return;

                _search = value;

                if (SelectedApplication != null)
                    _filterLastViewName = SelectedApplication.Name;

                Applications.Refresh();

                var sourceCollection = Applications.SourceCollection.Cast<ApplicationInfo>();
                var filteredCollection = Applications.Cast<ApplicationInfo>();

                var sourceInfos = sourceCollection as ApplicationInfo[] ?? sourceCollection.ToArray();
                var filteredInfos = filteredCollection as ApplicationInfo[] ?? filteredCollection.ToArray();

                if (_filterLastCount == null)
                    _filterLastCount = sourceInfos.Length;

                SelectedApplication = _filterLastCount > filteredInfos.Length ? filteredInfos.FirstOrDefault() : sourceInfos.FirstOrDefault(x => x.Name == _filterLastViewName);

                _filterLastCount = filteredInfos.Length;

                // Show note when there was nothing found
                SearchNothingFound = filteredInfos.Length == 0;

                OnPropertyChanged();
            }
        }

        private bool _searchNothingFound;
        public bool SearchNothingFound
        {
            get => _searchNothingFound;
            set
            {
                if (value == _searchNothingFound)
                    return;

                _searchNothingFound = value;
                OnPropertyChanged();
            }
        }

        private SettingsView _settingsView;

        private bool _showSettingsView;
        public bool ShowSettingsView
        {
            get => _showSettingsView;
            set
            {
                if (value == _showSettingsView)
                    return;

                _showSettingsView = value;
                OnPropertyChanged();
            }
        }

        private bool _isRestartRequired;
        public bool IsRestartRequired
        {
            get => _isRestartRequired;
            set
            {
                if (value == _isRestartRequired)
                    return;

                _isRestartRequired = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpdateAvailable;
        public bool IsUpdateAvailable
        {
            get => _isUpdateAvailable;
            set
            {
                if (value == _isUpdateAvailable)
                    return;

                _isUpdateAvailable = value;
                OnPropertyChanged();
            }
        }

        private string _updateReleaseUrl;
        public string UpdateReleaseUrl
        {
            get => _updateReleaseUrl;
            set
            {
                if (value == _updateReleaseUrl)
                    return;

                _updateReleaseUrl = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _profileFiles;
        public ICollectionView ProfileFiles
        {
            get => _profileFiles;
            set
            {
                if (value == _profileFiles)
                    return;

                _profileFiles = value;
                OnPropertyChanged();
            }
        }

        private ProfileFileInfo _selectedProfileFile = null;
        public ProfileFileInfo SelectedProfileFile
        {
            get => _selectedProfileFile;
            set
            {
                if (_isProfileLoading)
                    return;

                if (value != null && value.Equals(_selectedProfileFile))
                    return;

                _selectedProfileFile = value;

                if (value != null)
                {
                    if (!_isProfileUpdating)
                        LoadProfile(value);

                    if (SettingsManager.Current.Profiles_LastSelected != value.Name)
                        SettingsManager.Current.Profiles_LastSelected = value.Name;
                }

                OnPropertyChanged();
            }
        }

        private bool _isProfileFileDropDownOpened;
        public bool IsProfileFileDropDownOpened
        {
            get => _isProfileFileDropDownOpened;
            set
            {
                if (value == _isProfileFileDropDownOpened)
                    return;

                _isProfileFileDropDownOpened = value;
                OnPropertyChanged();
            }
        }

        private bool _isProfileFileLocked;
        public bool IsProfileFileLocked
        {
            get => _isProfileFileLocked;
            set
            {
                if (value == _isProfileFileLocked)
                    return;

                _isProfileFileLocked = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, window load and close events
        public MainWindow()
        {
            _isLoading = true;

            InitializeComponent();
            DataContext = this;

            // Language Meta
            LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(LocalizationManager.GetInstance().Culture.IetfLanguageTag)));

            // Load and change appearance
            AppearanceManager.Load();

            // Set window title
            Title = $"NETworkManager {AssemblyManager.Current.Version}";

            // Load settings
            ExpandApplicationView = SettingsManager.Current.ExpandApplicationView;

            // Register event system...
            SettingsManager.Current.PropertyChanged += SettingsManager_PropertyChanged;
            //EventSystem.RedirectProfileToApplicationEvent += EventSystem_RedirectProfileToApplicationEvent;
            EventSystem.OnRedirectDataToApplicationEvent += EventSystem_RedirectDataToApplicationEvent;
            EventSystem.OnRedirectToSettingsEvent += EventSystem_RedirectToSettingsEvent;

            _isLoading = false;
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Show a note if settings have been reset
            if (ConfigurationManager.Current.ShowSettingsResetNoteOnStartup)
            {
                var settings = AppearanceManager.MetroDialog;
                settings.AffirmativeButtonText = Localization.Resources.Strings.OK;

                ConfigurationManager.Current.FixAirspace = true;

                await this.ShowMessageAsync(Localization.Resources.Strings.SettingsHaveBeenReset, Localization.Resources.Strings.SettingsFileFoundWasCorruptOrNotCompatibleMessage, MessageDialogStyle.Affirmative, settings);

                ConfigurationManager.Current.FixAirspace = false;
            }

            // Show a note on the first run
            if (SettingsManager.Current.FirstRun)
            {
                // Show first run dialog...
                var customDialog = new CustomDialog
                {
                    Title = Localization.Resources.Strings.Welcome
                };

                var firstRunViewModel = new FirstRunViewModel(async instance =>
                {
                    await this.HideMetroDialogAsync(customDialog);

                    SettingsManager.Current.FirstRun = false;

                    // Set settings based on user choice
                    SettingsManager.Current.Update_CheckForUpdatesAtStartup = instance.CheckForUpdatesAtStartup;
                    SettingsManager.Current.Dashboard_CheckPublicIPAddress = instance.CheckPublicIPAddress;

                    // Generate lists at runtime
                    SettingsManager.Current.General_ApplicationList = new ObservableSetCollection<ApplicationInfo>(ApplicationManager.GetList());
                    SettingsManager.Current.IPScanner_CustomCommands = new ObservableCollection<CustomCommandInfo>(IPScannerCustomCommand.GetDefaultList());
                    SettingsManager.Current.PortScanner_PortProfiles = new ObservableCollection<PortProfileInfo>(PortProfile.GetDefaultList());
                    SettingsManager.Current.DNSLookup_DNSServers = new ObservableCollection<DNSServerInfo>(DNSServer.GetDefaultList());
                    SettingsManager.Current.AWSSessionManager_AWSProfiles = new ObservableCollection<AWSProfileInfo>(AWSProfile.GetDefaultList());

                    // Check if PuTTY is installed
                    foreach (var file in Models.PuTTY.PuTTY.GetDefaultInstallationPaths)
                    {
                        if (File.Exists(file))
                        {
                            SettingsManager.Current.PuTTY_ApplicationFilePath = file;
                            break;
                        }
                    }

                    // Save it to create a settings file
                    SettingsManager.Save();

                    AfterContentRendered();
                });

                customDialog.Content = new FirstRunDialog
                {
                    DataContext = firstRunViewModel
                };

                await this.ShowMetroDialogAsync(customDialog).ConfigureAwait(true);
            }
            else
            {
                AfterContentRendered();
            }
        }

        private void AfterContentRendered()
        {
            // Load the profiles before the applications are loaded so that we can use them (e.g. for synchronization)
            LoadProfiles();

            // Load application list, filter, sort, etc.
            LoadApplicationList();

            // Init notify icon
            if (SettingsManager.Current.TrayIcon_AlwaysShowIcon)
                InitNotifyIcon();

            // Hide to tray after the window shows up... not nice, but otherwise the hotkeys do not work
            if (CommandLineManager.Current.Autostart && SettingsManager.Current.Autostart_StartMinimizedInTray)
                HideWindowToTray();

            // Init status window
            _statusWindow = new StatusWindow(this);

            // Detect if network address or status changed...
            NetworkChange.NetworkAvailabilityChanged += (sender, args) => OnNetworkHasChanged();
            NetworkChange.NetworkAddressChanged += (sender, args) => OnNetworkHasChanged();

            // Set PowerShell global profile
            WriteDefaultPowerShellProfileToRegistry();

            // Search for updates... 
            if (SettingsManager.Current.Update_CheckForUpdatesAtStartup)
                CheckForUpdates();
        }
        private async void MetroWindowMain_Closing(object sender, CancelEventArgs e)
        {
            // Force restart --> e.g. Import or reset settings
            // Restart --> e.g. change profile path
            if (!_closeApplication && ConfigurationManager.Current.Restart)
            {
                e.Cancel = true;

                RestartApplication();

                return;
            }

            // Hide the application to tray
            if (!_closeApplication && (SettingsManager.Current.Window_MinimizeInsteadOfTerminating && WindowState != WindowState.Minimized))
            {
                e.Cancel = true;

                WindowState = WindowState.Minimized;

                return;
            }

            // Confirm close
            if (!_closeApplication && SettingsManager.Current.Window_ConfirmClose)
            {
                e.Cancel = true;

                // If the window is minimized, bring it to front
                if (WindowState == WindowState.Minimized)
                    BringWindowToFront();

                var settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = Localization.Resources.Strings.Close;
                settings.NegativeButtonText = Localization.Resources.Strings.Cancel;
                settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

                // Fix airspace issues
                ConfigurationManager.Current.FixAirspace = true;

                var result = await this.ShowMessageAsync(Localization.Resources.Strings.Confirm, Localization.Resources.Strings.ConfirmCloseMessage, MessageDialogStyle.AffirmativeAndNegative, settings);

                ConfigurationManager.Current.FixAirspace = false;

                if (result != MessageDialogResult.Affirmative)
                    return;

                _closeApplication = true;
                Close();

                return;
            }

            // Unregister HotKeys
            if (_registeredHotKeys.Count > 0)
                UnregisterHotKeys();

            // Dispose the notify icon to prevent errors
            _notifyIcon?.Dispose();
        }
        #endregion

        #region Application
        private void LoadApplicationList()
        {
            _isApplicationListLoading = true;

            Applications = new CollectionViewSource { Source = SettingsManager.Current.General_ApplicationList }.View;
            Applications.SortDescriptions.Add(new SortDescription(nameof(ApplicationInfo.Name), ListSortDirection.Ascending));

            Applications.Filter = o =>
            {
                if (o is not ApplicationInfo info)
                    return false;

                if (string.IsNullOrEmpty(Search))
                    return info.IsVisible;

                var regex = new Regex(@" |-");

                var search = regex.Replace(Search, "");

                // Search by TranslatedName and Name
                return info.IsVisible && (regex.Replace(ApplicationNameTranslator.GetInstance().Translate(info.Name), "").IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || regex.Replace(info.Name.ToString(), "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            };

            SettingsManager.Current.General_ApplicationList.CollectionChanged += (sender, args) => Applications.Refresh();

            _isApplicationListLoading = false;

            // Select the application
            SelectedApplication = Applications.SourceCollection.Cast<ApplicationInfo>().FirstOrDefault(x => x.Name == (CommandLineManager.Current.Application != ApplicationName.None ? CommandLineManager.Current.Application : SettingsManager.Current.General_DefaultApplicationViewName));

            // Scroll into view
            if (SelectedApplication != null)
                ListViewApplication.ScrollIntoView(SelectedApplication);
        }

        private DashboardView _dashboardView;
        private NetworkInterfaceView _networkInterfaceView;
        private WiFiView _wiFiView;
        private IPScannerHostView _ipScannerHostView;
        private PortScannerHostView _portScannerHostView;
        private PingMonitorHostView _pingMonitorHostView;
        private TracerouteHostView _tracerouteHostView;
        private DNSLookupHostView _dnsLookupHostView;
        private RemoteDesktopHostView _remoteDesktopHostView;
        private PowerShellHostView _powerShellHostView;
        private PuTTYHostView _puttyHostView;
        private AWSSessionManagerHostView _awsSessionManagerHostView;
        private TigerVNCHostView _tigerVNCHostView;
        private WebConsoleHostView _webConsoleHostView;
        private SNMPHostView _snmpHostView;
        private DiscoveryProtocolView _discoveryProtocolView;
        private WakeOnLANView _wakeOnLanView;
        private SubnetCalculatorHostView _subnetCalculatorHostView;
        private LookupHostView _lookupHostView;
        private WhoisHostView _whoisHostView;
        private ConnectionsView _connectionsView;
        private ListenersView _listenersView;
        private ARPTableView _arpTableView;


        /// <summary>
        /// Method when the application view becomes visible (again). Either when switching the applications 
        /// or after opening and closing the settings.
        /// </summary>
        /// <param name="name">Name of the application</param>
        /// <param name="fromSettings">Indicates whether the settings were previously open</param>
        private void OnApplicationViewVisible(ApplicationName name, bool fromSettings = false)
        {
            switch (name)
            {
                case ApplicationName.Dashboard:
                    if (_dashboardView == null)
                        _dashboardView = new DashboardView();
                    else
                        _dashboardView.OnViewVisible();

                    ContentControlApplication.Content = _dashboardView;
                    break;
                case ApplicationName.NetworkInterface:
                    if (_networkInterfaceView == null)
                        _networkInterfaceView = new NetworkInterfaceView();
                    else
                        _networkInterfaceView.OnViewVisible();

                    ContentControlApplication.Content = _networkInterfaceView;
                    break;
                case ApplicationName.WiFi:
                    if (_wiFiView == null)
                        _wiFiView = new WiFiView();
                    else
                        _wiFiView.OnViewVisible();

                    ContentControlApplication.Content = _wiFiView;
                    break;
                case ApplicationName.IPScanner:
                    if (_ipScannerHostView == null)
                        _ipScannerHostView = new IPScannerHostView();
                    else
                        _ipScannerHostView.OnViewVisible();

                    ContentControlApplication.Content = _ipScannerHostView;
                    break;
                case ApplicationName.PortScanner:
                    if (_portScannerHostView == null)
                        _portScannerHostView = new PortScannerHostView();
                    else
                        _portScannerHostView.OnViewVisible();

                    ContentControlApplication.Content = _portScannerHostView;
                    break;
                case ApplicationName.PingMonitor:
                    if (_pingMonitorHostView == null)
                        _pingMonitorHostView = new PingMonitorHostView();
                    else
                        _pingMonitorHostView.OnViewVisible();

                    ContentControlApplication.Content = _pingMonitorHostView;
                    break;
                case ApplicationName.Traceroute:
                    if (_tracerouteHostView == null)
                        _tracerouteHostView = new TracerouteHostView();
                    else
                        _tracerouteHostView.OnViewVisible();

                    ContentControlApplication.Content = _tracerouteHostView;
                    break;
                case ApplicationName.DNSLookup:
                    if (_dnsLookupHostView == null)
                        _dnsLookupHostView = new DNSLookupHostView();
                    else
                        _dnsLookupHostView.OnViewVisible();

                    ContentControlApplication.Content = _dnsLookupHostView;
                    break;
                case ApplicationName.RemoteDesktop:
                    if (_remoteDesktopHostView == null)
                        _remoteDesktopHostView = new RemoteDesktopHostView();
                    else
                        _remoteDesktopHostView.OnViewVisible();

                    ContentControlApplication.Content = _remoteDesktopHostView;
                    break;
                case ApplicationName.PowerShell:
                    if (_powerShellHostView == null)
                        _powerShellHostView = new PowerShellHostView();
                    else
                        _powerShellHostView.OnViewVisible();

                    ContentControlApplication.Content = _powerShellHostView;
                    break;
                case ApplicationName.PuTTY:
                    if (_puttyHostView == null)
                        _puttyHostView = new PuTTYHostView();
                    else
                        _puttyHostView.OnViewVisible();

                    ContentControlApplication.Content = _puttyHostView;
                    break;
                case ApplicationName.AWSSessionManager:
                    if (_awsSessionManagerHostView == null)
                        _awsSessionManagerHostView = new AWSSessionManagerHostView();
                    else
                        _awsSessionManagerHostView.OnViewVisible(fromSettings);

                    ContentControlApplication.Content = _awsSessionManagerHostView;
                    break;
                case ApplicationName.TigerVNC:
                    if (_tigerVNCHostView == null)
                        _tigerVNCHostView = new TigerVNCHostView();
                    else
                        _tigerVNCHostView.OnViewVisible();

                    ContentControlApplication.Content = _tigerVNCHostView;
                    break;
                case ApplicationName.WebConsole:
                    if (_webConsoleHostView == null)
                        _webConsoleHostView = new WebConsoleHostView();
                    else
                        _webConsoleHostView.OnViewVisible();

                    ContentControlApplication.Content = _webConsoleHostView;
                    break;
                case ApplicationName.SNMP:
                    if (_snmpHostView == null)
                        _snmpHostView = new SNMPHostView();
                    else
                        _snmpHostView.OnViewVisible();

                    ContentControlApplication.Content = _snmpHostView;
                    break;
                case ApplicationName.DiscoveryProtocol:
                    if (_discoveryProtocolView == null)
                        _discoveryProtocolView = new DiscoveryProtocolView();
                    else
                        _discoveryProtocolView.OnViewVisible();

                    ContentControlApplication.Content = _discoveryProtocolView;
                    break;
                case ApplicationName.WakeOnLAN:
                    if (_wakeOnLanView == null)
                        _wakeOnLanView = new WakeOnLANView();
                    else
                        _wakeOnLanView.OnViewVisible();

                    ContentControlApplication.Content = _wakeOnLanView;
                    break;
                case ApplicationName.Whois:
                    if (_whoisHostView == null)
                        _whoisHostView = new WhoisHostView();
                    else
                        _whoisHostView.OnViewVisible();

                    ContentControlApplication.Content = _whoisHostView;
                    break;
                case ApplicationName.SubnetCalculator:
                    if (_subnetCalculatorHostView == null)
                        _subnetCalculatorHostView = new SubnetCalculatorHostView();
                    //else
                    //    _subnetCalculatorHostView.OnViewVisible();

                    ContentControlApplication.Content = _subnetCalculatorHostView;
                    break;
                case ApplicationName.Lookup:
                    if (_lookupHostView == null)
                        _lookupHostView = new LookupHostView();
                    //else
                    //    _lookupHostView.OnViewVisible();

                    ContentControlApplication.Content = _lookupHostView;
                    break;
                case ApplicationName.Connections:
                    if (_connectionsView == null)
                        _connectionsView = new ConnectionsView();
                    else
                        _connectionsView.OnViewVisible();

                    ContentControlApplication.Content = _connectionsView;
                    break;
                case ApplicationName.Listeners:
                    if (_listenersView == null)
                        _listenersView = new ListenersView();
                    else
                        _listenersView.OnViewVisible();

                    ContentControlApplication.Content = _listenersView;
                    break;
                case ApplicationName.ARPTable:
                    if (_arpTableView == null)
                        _arpTableView = new ARPTableView();
                    else
                        _arpTableView.OnViewVisible();

                    ContentControlApplication.Content = _arpTableView;
                    break;
            }
        }

        private void OnApplicationViewHide(ApplicationName name)
        {
            switch (name)
            {
                case ApplicationName.Dashboard:
                    _dashboardView?.OnViewHide();
                    break;
                case ApplicationName.NetworkInterface:
                    _networkInterfaceView?.OnViewHide();
                    break;
                case ApplicationName.WiFi:
                    _wiFiView?.OnViewHide();
                    break;
                case ApplicationName.IPScanner:
                    _ipScannerHostView?.OnViewHide();
                    break;
                case ApplicationName.PortScanner:
                    _portScannerHostView?.OnViewHide();
                    break;
                case ApplicationName.PingMonitor:
                    _pingMonitorHostView?.OnViewHide();
                    break;
                case ApplicationName.Traceroute:
                    _tracerouteHostView?.OnViewHide();
                    break;
                case ApplicationName.DNSLookup:
                    _dnsLookupHostView?.OnViewHide();
                    break;
                case ApplicationName.RemoteDesktop:
                    _remoteDesktopHostView?.OnViewHide();
                    break;
                case ApplicationName.PowerShell:
                    _powerShellHostView?.OnViewHide();
                    break;
                case ApplicationName.PuTTY:
                    _puttyHostView?.OnViewHide();
                    break;
                case ApplicationName.AWSSessionManager:
                    _awsSessionManagerHostView?.OnViewHide();
                    break;
                case ApplicationName.TigerVNC:
                    _tigerVNCHostView?.OnViewHide();
                    break;
                case ApplicationName.WebConsole:
                    _webConsoleHostView?.OnViewHide();
                    break;
                case ApplicationName.SNMP:
                    _snmpHostView?.OnViewHide();
                    break;
                case ApplicationName.DiscoveryProtocol:
                    _discoveryProtocolView?.OnViewHide();
                    break;
                case ApplicationName.WakeOnLAN:
                    _wakeOnLanView?.OnViewHide();
                    break;
                //case ApplicationName.Lookup:
                //    _lookupHostView?.OnViewHide();
                //    break;
                //case ApplicationName.SubnetCalculator:
                //    _subnetCalculatorHostView?.OnViewHide();
                //    break;
                case ApplicationName.Connections:
                    _connectionsView?.OnViewHide();
                    break;
                case ApplicationName.Listeners:
                    _listenersView?.OnViewHide();
                    break;
                case ApplicationName.ARPTable:
                    _arpTableView?.OnViewHide();
                    break;
            }
        }

        private void ClearSearchOnApplicationListMinimize()
        {
            if (ExpandApplicationView)
                return;

            if (IsApplicationListOpen && IsTextBoxSearchFocused)
                return;

            if (IsApplicationListOpen && IsMouseOverApplicationList)
                return;

            Search = string.Empty;

            // Scroll into view
            ListViewApplication.ScrollIntoView(SelectedApplication);
        }

        private void EventSystem_RedirectDataToApplicationEvent(object sender, EventArgs e)
        {
            if (e is not EventSystemRedirectArgs data)
                return;

            // Change view
            SelectedApplication = Applications.SourceCollection.Cast<ApplicationInfo>().FirstOrDefault(x => x.Name == data.Application);

            // Crate a new tab / perform action
            switch (data.Application)
            {
                case ApplicationName.Dashboard:
                    break;
                case ApplicationName.NetworkInterface:
                    break;
                case ApplicationName.WiFi:
                    break;
                case ApplicationName.IPScanner:
                    _ipScannerHostView.AddTab(data.Args);
                    break;
                case ApplicationName.PortScanner:
                    _portScannerHostView.AddTab(data.Args);
                    break;
                case ApplicationName.PingMonitor:
                    _pingMonitorHostView.AddHost(data.Args);
                    break;
                case ApplicationName.Traceroute:
                    _tracerouteHostView.AddTab(data.Args);
                    break;
                case ApplicationName.DNSLookup:
                    _dnsLookupHostView.AddTab(data.Args);
                    break;
                case ApplicationName.RemoteDesktop:
                    _remoteDesktopHostView.AddTab(data.Args);
                    break;
                case ApplicationName.PowerShell:
                    _powerShellHostView.AddTab(data.Args);
                    break;
                case ApplicationName.PuTTY:
                    _puttyHostView.AddTab(data.Args);
                    break;
                case ApplicationName.AWSSessionManager:
                    break;
                case ApplicationName.TigerVNC:
                    _tigerVNCHostView.AddTab(data.Args);
                    break;
                case ApplicationName.WebConsole:
                    break;
                case ApplicationName.SNMP:
                    _snmpHostView.AddTab(data.Args);
                    break;
                case ApplicationName.DiscoveryProtocol:
                    break;
                case ApplicationName.WakeOnLAN:
                    break;
                case ApplicationName.Whois:
                    break;
                case ApplicationName.SubnetCalculator:
                    break;
                case ApplicationName.Lookup:
                    break;
                case ApplicationName.Connections:
                    break;
                case ApplicationName.Listeners:
                    break;
                case ApplicationName.ARPTable:
                    break;
                case ApplicationName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Settings
        private void OpenSettings()
        {
            // Init settings view
            if (_settingsView == null)
            {
                _settingsView = new SettingsView(SelectedApplication.Name);
                ContentControlSettings.Content = _settingsView;
            }
            else // Change view
            {
                _settingsView.ChangeSettingsView(SelectedApplication.Name);
                _settingsView.Refresh();
            }

            // Show the view (this will hide other content)
            ShowSettingsView = true;
        }

        private void EventSystem_RedirectToSettingsEvent(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private async Task CloseSettings()
        {
            ShowSettingsView = false;

            // Change HotKeys
            if (SettingsManager.HotKeysChanged)
            {
                UnregisterHotKeys();
                RegisterHotKeys();

                SettingsManager.HotKeysChanged = false;
            }

            // Refresh the application view
            OnApplicationViewVisible(SelectedApplication.Name, true);
        }
        #endregion

        #region Profiles
        private void LoadProfiles()
        {
            _isProfileLoading = true;
            ProfileFiles = new CollectionViewSource { Source = ProfileManager.ProfileFiles }.View;
            ProfileFiles.SortDescriptions.Add(new SortDescription(nameof(ProfileFileInfo.Name), ListSortDirection.Ascending));
            _isProfileLoading = false;

            ProfileManager.OnLoadedProfileFileChangedEvent += ProfileManager_OnLoadedProfileFileChangedEvent;
            ProfileManager.OnSwitchProfileFileViaUIEvent += ProfileManager_OnSwitchProfileFileViaUIEvent;

            SelectedProfileFile = ProfileFiles.SourceCollection.Cast<ProfileFileInfo>().FirstOrDefault(x => x.Name == SettingsManager.Current.Profiles_LastSelected);
            SelectedProfileFile ??= ProfileFiles.SourceCollection.Cast<ProfileFileInfo>().FirstOrDefault();
        }

        private async Task LoadProfile(ProfileFileInfo info)
        {
            if (info.IsEncrypted && !info.IsPasswordValid)
            {
                IsProfileFileLocked = true;

                var customDialog = new CustomDialog
                {
                    Title = Localization.Resources.Strings.MasterPassword
                };

                var credentialsPasswordViewModel = new CredentialsPasswordViewModel(async instance =>
                {
                    await this.HideMetroDialogAsync(customDialog);
                    ConfigurationManager.Current.FixAirspace = false;

                    info.Password = instance.Password;

                    SwitchProfile(info);
                }, async instance =>
                {
                    await this.HideMetroDialogAsync(customDialog);
                    ConfigurationManager.Current.FixAirspace = false;

                    ProfileManager.Unload();
                });

                customDialog.Content = new CredentialsPasswordDialog
                {
                    DataContext = credentialsPasswordViewModel
                };

                ConfigurationManager.Current.FixAirspace = true;
                await this.ShowMetroDialogAsync(customDialog);
            }
            else
            {
                SwitchProfile(info);
            }
        }

        private async Task SwitchProfile(ProfileFileInfo info)
        {
            try
            {
                ProfileManager.Switch(info);

                IsProfileFileLocked = false;

                // Null if profile is loaded before application is loaded
                if (SelectedApplication != null)
                    OnProfilesLoaded(SelectedApplication.Name);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                var settings = AppearanceManager.MetroDialog;
                settings.AffirmativeButtonText = Localization.Resources.Strings.OK;

                ConfigurationManager.Current.FixAirspace = true;

                await this.ShowMessageAsync(Localization.Resources.Strings.WrongPassword, Localization.Resources.Strings.WrongPasswordDecryptionFailedMessage, MessageDialogStyle.Affirmative, settings);

                ConfigurationManager.Current.FixAirspace = false;
            }
            catch
            {
                var settings = AppearanceManager.MetroDialog;
                settings.AffirmativeButtonText = Localization.Resources.Strings.Migrate;
                settings.NegativeButtonText = Localization.Resources.Strings.Cancel;
                settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

                ConfigurationManager.Current.FixAirspace = true;

                // ToDo: Improve Message
                var result = await this.ShowMessageAsync(Localization.Resources.Strings.ProfileCouldNotBeLoaded, Localization.Resources.Strings.ProfileCouldNotBeLoadedMessage, MessageDialogStyle.AffirmativeAndNegative, settings);

                ConfigurationManager.Current.FixAirspace = false;

                if (result == MessageDialogResult.Affirmative)
                {
                    ExternalProcessStarter.RunProcess("powershell.exe", $"-NoLogo -NoProfile -ExecutionPolicy ByPass -File \"{Path.Combine(ConfigurationManager.Current.ExecutionPath, "Resources", "Migrate-Profiles.ps1")}\" -Path \"{ProfileManager.GetProfilesLocation()}\" -NETworkManagerPath \"{ConfigurationManager.Current.ApplicationFullName}\" -NETworkManagerVersion \"{AssemblyManager.Current.Version}\"");
                    CloseApplication();
                }
            }
        }

        private void OnProfilesLoaded(ApplicationName name)
        {
            switch (name)
            {
                case ApplicationName.AWSSessionManager:
                    _awsSessionManagerHostView?.OnProfileLoaded();
                    break;
            }
        }

        /// <summary>
        /// Update the view when the loaded profile file changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileManager_OnLoadedProfileFileChangedEvent(object sender, ProfileFileInfoArgs e)
        {
            _isProfileUpdating = true;

            SelectedProfileFile = ProfileFiles.SourceCollection.Cast<ProfileFileInfo>().FirstOrDefault(x => x.Equals(e.ProfileFileInfo));

            _isProfileUpdating = false;
        }

        /// <summary>
        /// Switch the profile from code behind via UI to get the password for encrypted files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileManager_OnSwitchProfileFileViaUIEvent(object sender, ProfileFileInfoArgs e)
        {
            SelectedProfileFile = ProfileFiles.SourceCollection.Cast<ProfileFileInfo>().FirstOrDefault(x => x.Equals(e.ProfileFileInfo));
        }
        #endregion

        #region Update check
        private void CheckForUpdates()
        {
            var updater = new Updater();

            updater.UpdateAvailable += Updater_UpdateAvailable;
            updater.Error += Updater_Error;
            updater.CheckOnGitHub(Properties.Resources.NETworkManager_GitHub_User, Properties.Resources.NETworkManager_GitHub_Repo, AssemblyManager.Current.Version, SettingsManager.Current.Update_CheckForPreReleases);
        }

        private static void Updater_Error(object sender, EventArgs e)
        {
            //  Log
        }

        private void Updater_UpdateAvailable(object sender, UpdateAvailableArgs e)
        {
            UpdateReleaseUrl = e.Release.Prerelease ? e.Release.HtmlUrl : Properties.Resources.NETworkManager_LatestReleaseUrl;
            IsUpdateAvailable = true;
        }
        #endregion

        #region Handle WndProc messages (Single instance, handle HotKeys)
        private HwndSource _hwndSoure;

        // This is called after MainWindow() and before OnContentRendered() --> to register hotkeys...
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwndSoure = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            _hwndSoure?.AddHook(HwndHook);

            RegisterHotKeys();
        }

        [DebuggerStepThrough]
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Single instance or Hotkey --> Show window
            if (msg == SingleInstance.WM_SHOWME || msg == WmHotkey && wParam.ToInt32() == 1)
            {
                ShowWindow();
                handled = true;
            }

            return IntPtr.Zero;
        }
        #endregion

        #region Global HotKeys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // WM_HOTKEY
        private const int WmHotkey = 0x0312;

        /* ID | Command
        *  ---|-------------------
        *  1  | ShowWindow()
        */

        private readonly List<int> _registeredHotKeys = new();

        private void RegisterHotKeys()
        {
            if (SettingsManager.Current.HotKey_ShowWindowEnabled)
            {
                RegisterHotKey(new WindowInteropHelper(this).Handle, 1, SettingsManager.Current.HotKey_ShowWindowModifier, SettingsManager.Current.HotKey_ShowWindowKey);
                _registeredHotKeys.Add(1);
            }
        }

        private void UnregisterHotKeys()
        {
            // Unregister all registred keys
            foreach (var i in _registeredHotKeys)
                UnregisterHotKey(new WindowInteropHelper(this).Handle, i);

            // Clear list
            _registeredHotKeys.Clear();
        }
        #endregion

        #region NotifyIcon
        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();

            // Get the application icon for the tray
            using (var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/NETworkManager.ico"))?.Stream)
            {
                if (iconStream != null)
                    _notifyIcon.Icon = new Icon(iconStream);
            }

            _notifyIcon.Text = Title;
            _notifyIcon.Click += NotifyIcon_Click;
            _notifyIcon.MouseDown += NotifyIcon_MouseDown;
            _notifyIcon.Visible = true;
        }

        private void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            var trayMenu = (ContextMenu)FindResource("ContextMenuNotifyIcon");

            trayMenu.IsOpen = true;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MouseEventArgs mouse = (System.Windows.Forms.MouseEventArgs)e;

            if (mouse.Button != MouseButtons.Left)
                return;

            if (OpenStatusWindowCommand.CanExecute(null))
                OpenStatusWindowCommand.Execute(null);
        }

        private void MetroWindowMain_StateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Minimized)
                return;

            if (SettingsManager.Current.Window_MinimizeToTrayInsteadOfTaskbar)
                HideWindowToTray();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.DataContext = this;
        }
        #endregion

        #region ICommands & Actions
        public ICommand OpenStatusWindowCommand => new RelayCommand(p => OpenStatusWindowAction());

        private void OpenStatusWindowAction()
        {
            OpenStatusWindow(true);
        }

        public ICommand RestartApplicationCommand => new RelayCommand(p => RestartApplicationAction());

        private void RestartApplicationAction()
        {
            RestartApplication();
        }

        public ICommand OpenWebsiteCommand => new RelayCommand(OpenWebsiteAction);

        private static void OpenWebsiteAction(object url)
        {
            ExternalProcessStarter.OpenUrl((string)url);
        }

        public ICommand OpenDocumentationCommand => new RelayCommand(p => OpenDocumentationAction());

        private void OpenDocumentationAction()
        {
            DocumentationManager.OpenDocumentation(ShowSettingsView ? _settingsView.GetDocumentationIdentifier() : DocumentationManager.GetIdentifierByAppliactionName(SelectedApplication.Name));
        }

        public ICommand OpenApplicationListCommand => new RelayCommand(p => OpenApplicationListAction());

        private void OpenApplicationListAction()
        {
            IsApplicationListOpen = true;
            TextBoxSearch.Focus();
        }

        public ICommand UnlockProfileCommand => new RelayCommand(p => UnlockProfileAction());

        private void UnlockProfileAction()
        {
            LoadProfile(SelectedProfileFile);
        }

        public ICommand OpenSettingsCommand => new RelayCommand(p => OpenSettingsAction());

        private void OpenSettingsAction()
        {
            OpenSettings();
        }

        public ICommand OpenSettingsFromTrayCommand => new RelayCommand(p => OpenSettingsFromTrayAction());

        private void OpenSettingsFromTrayAction()
        {
            // Bring window to front
            ShowWindow();

            OpenSettings();
        }

        public ICommand CloseSettingsCommand => new RelayCommand(p => CloseSettingsAction());

        private void CloseSettingsAction()
        {
            CloseSettings();
        }

        public ICommand ShowWindowCommand => new RelayCommand(p => ShowWindowAction());

        private void ShowWindowAction()
        {
            ShowWindow();
        }

        public ICommand CloseApplicationCommand => new RelayCommand(p => CloseApplicationAction());

        private void CloseApplicationAction()
        {
            CloseApplication();
        }

        public void CloseApplication()
        {
            _closeApplication = true;

            // Make it thread safe when it's called inside a dialog
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                Close();
            }));
        }

        public void RestartApplication(bool asAdmin = false)
        {
            ExternalProcessStarter.RunProcess(ConfigurationManager.Current.ApplicationFullName, $"{CommandLineManager.GetParameterWithSplitIdentifier(CommandLineManager.ParameterRestartPid)}{Process.GetCurrentProcess().Id} {CommandLineManager.GetParameterWithSplitIdentifier(CommandLineManager.ParameterApplication)}{SelectedApplication.Name}", asAdmin);

            CloseApplication();
        }

        public ICommand ApplicationListMouseEnterCommand => new RelayCommand(p => ApplicationListMouseEnterAction());

        private void ApplicationListMouseEnterAction()
        {
            IsMouseOverApplicationList = true;
        }

        public ICommand ApplicationListMouseLeaveCommand => new RelayCommand(p => ApplicationListMouseLeaveAction());

        private void ApplicationListMouseLeaveAction()
        {
            // Don't minmize the list, if the user has accidently moved the mouse while searching
            if (!IsTextBoxSearchFocused)
                IsApplicationListOpen = false;

            IsMouseOverApplicationList = false;
        }

        public ICommand TextBoxSearchGotFocusCommand => new RelayCommand(p => TextBoxSearchGotFocusAction());

        private void TextBoxSearchGotFocusAction()
        {
            IsTextBoxSearchFocused = true;
        }

        public ICommand TextBoxSearchLostFocusCommand => new RelayCommand(p => TextBoxSearchLostFocusAction());

        private void TextBoxSearchLostFocusAction()
        {
            if (!IsMouseOverApplicationList)
                IsApplicationListOpen = false;

            IsTextBoxSearchFocused = false;
        }

        public ICommand ClearSearchCommand => new RelayCommand(p => ClearSearchAction());

        private void ClearSearchAction()
        {
            Search = string.Empty;
        }
        #endregion

        #region Methods
        private void ShowWindow()
        {
            if (_isInTray)
                ShowWindowFromTray();

            if (!IsActive)
                BringWindowToFront();
        }

        private void HideWindowToTray()
        {
            if (_notifyIcon == null)
                InitNotifyIcon();

            _isInTray = true;

            _notifyIcon.Visible = true;

            Hide();
        }

        private void ShowWindowFromTray()
        {
            _isInTray = false;

            Show();

            _notifyIcon.Visible = SettingsManager.Current.TrayIcon_AlwaysShowIcon;
        }

        private void BringWindowToFront()
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            Activate();
        }

        private void WriteDefaultPowerShellProfileToRegistry()
        {
            if (!SettingsManager.Current.Appearance_PowerShellModifyGlobalProfile)
                return;

            HashSet<string> paths = new();

            // PowerShell
            if (!string.IsNullOrEmpty(SettingsManager.Current.PowerShell_ApplicationFilePath) && File.Exists(SettingsManager.Current.PowerShell_ApplicationFilePath))
                paths.Add(SettingsManager.Current.PowerShell_ApplicationFilePath);

            // AWS Session Manager
            if (!string.IsNullOrEmpty(SettingsManager.Current.AWSSessionManager_ApplicationFilePath) && File.Exists(SettingsManager.Current.AWSSessionManager_ApplicationFilePath))
                paths.Add(SettingsManager.Current.AWSSessionManager_ApplicationFilePath);

            foreach (var path in paths)
                PowerShell.WriteDefaultProfileToRegistry(SettingsManager.Current.Appearance_Theme, path);
        }
        #endregion

        #region Status window
        private void OpenStatusWindow(bool activate)
        {
            _statusWindow.ShowWindow(activate);
        }

        private void OnNetworkHasChanged()
        {
            // Show status window on network change
            if (SettingsManager.Current.Status_ShowWindowOnNetworkChange)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    OpenStatusWindow(false);
                }));
            }
        }
        #endregion

        #region Events
        private void SettingsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsInfo.Localization_CultureCode))
                IsRestartRequired = true;

            if (e.PropertyName == nameof(SettingsInfo.TrayIcon_AlwaysShowIcon))
            {
                if (SettingsManager.Current.TrayIcon_AlwaysShowIcon && _notifyIcon == null)
                    InitNotifyIcon();

                if (_notifyIcon != null)
                    _notifyIcon.Visible = SettingsManager.Current.TrayIcon_AlwaysShowIcon;
            }

            if (e.PropertyName == nameof(SettingsInfo.Appearance_PowerShellModifyGlobalProfile) || e.PropertyName == nameof(SettingsInfo.Appearance_Theme) || e.PropertyName == nameof(SettingsInfo.PowerShell_ApplicationFilePath) || e.PropertyName == nameof(SettingsInfo.AWSSessionManager_ApplicationFilePath))
                WriteDefaultPowerShellProfileToRegistry();
        }
        #endregion

        #region Bugfixes
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Focus embedded window
        private void MetroMainWindow_Activated(object sender, EventArgs e)
        {
            FocusEmbeddedWindow();
        }

        private async void FocusEmbeddedWindow()
        {
            // Delay the focus to prevent blocking the ui
            do
            {
                await Task.Delay(250);
            } while (Control.MouseButtons == MouseButtons.Left);

            /* Don't continue if
               - Application is not set
               - Settings are opened
               - Profile file drop down is opened
               - Application search textbox is opened
               - Dialog over an embedded window is opened
               - Window is resizing
            */
            if (SelectedApplication == null || ShowSettingsView || IsProfileFileDropDownOpened || IsTextBoxSearchFocused || ConfigurationManager.Current.FixAirspace)
                return;

            // Switch by name
            switch (SelectedApplication.Name)
            {
                case ApplicationName.PowerShell:
                    _powerShellHostView?.FocusEmbeddedWindow();
                    break;
                case ApplicationName.PuTTY:
                    _puttyHostView?.FocusEmbeddedWindow();
                    break;
                case ApplicationName.AWSSessionManager:
                    _awsSessionManagerHostView?.FocusEmbeddedWindow();
                    break;
            }
        }
        #endregion
    }
}

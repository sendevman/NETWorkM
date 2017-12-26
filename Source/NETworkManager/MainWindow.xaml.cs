﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using NETworkManager.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Views.Applications;
using NETworkManager.Views.Settings;
using NETworkManager.Models.Settings;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.Generic;
using NETworkManager.Views;
using NETworkManager.Helpers;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace NETworkManager
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables        
        NotifyIcon notifyIcon;

        private bool _isLoading = true;

        private bool _isInTray;
        private bool _closeApplication;

        // Indicates a restart message, when settings changed
        private string _cultureCode;

        private bool _applicationView_Expand;
        public bool ApplicationView_Expand
        {
            get { return _applicationView_Expand; }
            set
            {
                if (value == _applicationView_Expand)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.ApplicationView_Expand = value;

                if (!value)
                    ClearSearchFilterOnApplicationListMinimize();

                _applicationView_Expand = value;
                OnPropertyChanged();
            }
        }

        private bool _isTextBoxSearchFocused;
        public bool IsTextBoxSearchFocused
        {
            get { return _isTextBoxSearchFocused; }
            set
            {
                if (value == _isTextBoxSearchFocused)
                    return;

                if (!value)
                    ClearSearchFilterOnApplicationListMinimize();

                _isTextBoxSearchFocused = value;
                OnPropertyChanged();
            }
        }

        private bool _openApplicationList;
        public bool OpenApplicationList
        {
            get { return _openApplicationList; }
            set
            {
                if (value == _openApplicationList)
                    return;

                if (!value)
                    ClearSearchFilterOnApplicationListMinimize();

                _openApplicationList = value;
                OnPropertyChanged();
            }
        }

        private bool _isMouseOverApplicationList;
        public bool IsMouseOverApplicationList
        {
            get { return _isMouseOverApplicationList; }
            set
            {
                if (value == _isMouseOverApplicationList)
                    return;

                if (!value)
                    ClearSearchFilterOnApplicationListMinimize();

                _isMouseOverApplicationList = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _applications;
        public ICollectionView Applications
        {
            get { return _applications; }
        }

        private ApplicationViewInfo _selectedApplication;
        public ApplicationViewInfo SelectedApplication
        {
            get { return _selectedApplication; }
            set
            {
                if (value == _selectedApplication)
                    return;

                if (value != null)
                    ChangeApplicationView(value.Name);

                _selectedApplication = value;
                OnPropertyChanged();
            }
        }

        ApplicationViewManager.Name filterLastViewName;
        int? filterLastCount;

        private string _search;
        public string Search
        {
            get { return _search; }
            set
            {
                if (value == _search)
                    return;

                _search = value;

                if (SelectedApplication != null)
                    filterLastViewName = SelectedApplication.Name;

                Applications.Refresh();

                IEnumerable<ApplicationViewInfo> sourceCollection = Applications.SourceCollection.Cast<ApplicationViewInfo>();
                IEnumerable<ApplicationViewInfo> filteredCollection = Applications.Cast<ApplicationViewInfo>();

                int sourceCollectionCount = sourceCollection.Count();
                int filteredCollectionCount = filteredCollection.Count();

                if (filterLastCount == null)
                    filterLastCount = sourceCollectionCount;

                if (filterLastCount > filteredCollectionCount)
                    SelectedApplication = filteredCollection.FirstOrDefault();
                else
                    SelectedApplication = sourceCollection.FirstOrDefault(x => x.Name == filterLastViewName);

                filterLastCount = filteredCollectionCount;

                // Show note when there was nothing found
                SearchNothingFound = filteredCollectionCount == 0;

                OnPropertyChanged();
            }
        }

        private bool _searchNothingFound;
        public bool SearchNothingFound
        {
            get { return _searchNothingFound; }
            set
            {
                if (value == _searchNothingFound)
                    return;

                _searchNothingFound = value;
                OnPropertyChanged();
            }
        }

        SettingsView _settingsView;

        private bool _showSettingsView;
        public bool ShowSettingsView
        {
            get { return _showSettingsView; }
            set
            {
                if (value == _showSettingsView)
                    return;

                _showSettingsView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, window load and close events
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Language Meta
            LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(LocalizationManager.Culture.IetfLanguageTag)));

            // Load appearance
            AppearanceManager.Load();

            // Transparency
            if (SettingsManager.Current.Appearance_EnableTransparency)
            {
                AllowsTransparency = true;
                Opacity = SettingsManager.Current.Appearance_Opacity;
            }

            // NotifyIcon for Autostart
            if (CommandLineManager.Current.Autostart && SettingsManager.Current.Autostart_StartMinimizedInTray || SettingsManager.Current.TrayIcon_AlwaysShowIcon)
                InitNotifyIcon();

            // Set windows title if admin
            if (ConfigurationManager.Current.IsAdmin)
                Title = string.Format("[{0}] {1}", System.Windows.Application.Current.Resources["String_Administrator"] as string, Title);

            // Load application list, filter, sort
            LoadApplicationList();

            // Load settings
            ApplicationView_Expand = SettingsManager.Current.ApplicationView_Expand;

            _isLoading = false;
        }

        // Hide window after it shows up... not nice, but otherwise the hotkeys do not work
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (CommandLineManager.Current.Autostart && SettingsManager.Current.Autostart_StartMinimizedInTray)
                HideWindowToTray();
        }

        private void LoadApplicationList()
        {
            _applications = CollectionViewSource.GetDefaultView(ApplicationViewManager.List);
            _applications.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending)); // Always have the same order, even if it is translated
            _applications.Filter = o =>
            {
                if (string.IsNullOrEmpty(Search))
                    return true;

                // Search for application name and description without "-" and " "
                ApplicationViewInfo info = o as ApplicationViewInfo;

                Regex regex = new Regex(@" |-");

                string search = regex.Replace(Search, "");

                // Search by TranslatedName and Name
                return (regex.Replace(info.TranslatedName, "").IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1) || (regex.Replace(info.Name.ToString(), "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            };

            // Get application from settings
            SelectedApplication = Applications.SourceCollection.Cast<ApplicationViewInfo>().FirstOrDefault(x => x.Name == SettingsManager.Current.Application_DefaultApplicationViewName);

            // Scroll into view
            listViewApplication.ScrollIntoView(SelectedApplication);
        }

        private async void MetroWindowMain_Closing(object sender, CancelEventArgs e)
        {
            // Force restart (if user has reset the settings or import them)
            if (SettingsManager.ForceRestart || ImportExportManager.ForceRestart)
            {
                RestartApplication(false);

                _closeApplication = true;
            }

            // Hide the application to tray
            if (!_closeApplication && SettingsManager.Current.Window_MinimizeInsteadOfTerminating)
            {
                e.Cancel = true;

                WindowState = WindowState.Minimized;

                return;
            }

            // Confirm close
            if (!_closeApplication && SettingsManager.Current.Window_ConfirmClose)
            {
                e.Cancel = true;

                MetroDialogSettings settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = System.Windows.Application.Current.Resources["String_Button_Close"] as string;
                settings.NegativeButtonText = System.Windows.Application.Current.Resources["String_Button_Cancel"] as string;
                settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

                // Fix airspace issues
                ConfigurationManager.Current.FixAirspace = true;

                MessageDialogResult result = await this.ShowMessageAsync(System.Windows.Application.Current.Resources["String_Header_Confirm"] as string, System.Windows.Application.Current.Resources["String_ConfirmCloseQuesiton"] as string, MessageDialogStyle.AffirmativeAndNegative, settings);

                ConfigurationManager.Current.FixAirspace = false;

                if (result == MessageDialogResult.Affirmative)
                {
                    _closeApplication = true;
                    Close();
                }

                return;
            }

            // Unregister HotKeys
            if (RegisteredHotKeys.Count > 0)
                UnregisterHotKeys();

            // Dispose the notify icon to prevent errors
            if (notifyIcon != null)
                notifyIcon.Dispose();
        }
        #endregion

        #region Application Views
        NetworkInterfaceView networkInterfaceView;
        IPScannerView ipScannerView;
        PortScannerView portScannerView;
        PingView pingView;
        TracerouteView tracerouteView;
        DNSLookupView dnsLookupView;
        RemoteDesktopView remoteDesktopView;
        WakeOnLANView wakeOnLANView;
        SubnetCalculatorView subnetCalculatorView;
        HTTPHeadersView httpHeadersView;
        ARPTableView arpTableView;
        LookupView lookupView;

        private ApplicationViewManager.Name? currentApplicationViewName = null;

        private void ChangeApplicationView(ApplicationViewManager.Name name)
        {
            if (currentApplicationViewName == name)
                return;

            switch (name)
            {
                case ApplicationViewManager.Name.NetworkInterface:
                    if (networkInterfaceView == null)
                        networkInterfaceView = new NetworkInterfaceView();

                    contentControlApplication.Content = networkInterfaceView;
                    break;
                case ApplicationViewManager.Name.IPScanner:
                    if (ipScannerView == null)
                        ipScannerView = new IPScannerView();

                    contentControlApplication.Content = ipScannerView;
                    break;
                case ApplicationViewManager.Name.PortScanner:
                    if (portScannerView == null)
                        portScannerView = new PortScannerView();

                    contentControlApplication.Content = portScannerView;
                    break;
                case ApplicationViewManager.Name.Ping:
                    if (pingView == null)
                        pingView = new PingView();

                    contentControlApplication.Content = pingView;
                    break;
                case ApplicationViewManager.Name.Traceroute:
                    if (tracerouteView == null)
                        tracerouteView = new TracerouteView();

                    contentControlApplication.Content = tracerouteView;
                    break;
                case ApplicationViewManager.Name.DNSLookup:
                    if (dnsLookupView == null)
                        dnsLookupView = new DNSLookupView();

                    contentControlApplication.Content = dnsLookupView;
                    break;
                case ApplicationViewManager.Name.RemoteDesktop:
                    if (remoteDesktopView == null)
                        remoteDesktopView = new RemoteDesktopView();

                    contentControlApplication.Content = remoteDesktopView;
                    break;
                case ApplicationViewManager.Name.WakeOnLAN:
                    if (wakeOnLANView == null)
                        wakeOnLANView = new WakeOnLANView();

                    contentControlApplication.Content = wakeOnLANView;
                    break;
                case ApplicationViewManager.Name.SubnetCalculator:
                    if (subnetCalculatorView == null)
                        subnetCalculatorView = new SubnetCalculatorView();

                    contentControlApplication.Content = subnetCalculatorView;
                    break;
                case ApplicationViewManager.Name.HTTPHeaders:
                    if (httpHeadersView == null)
                        httpHeadersView = new HTTPHeadersView();

                    contentControlApplication.Content = httpHeadersView;
                    break;
                case ApplicationViewManager.Name.ARPTable:
                    if (arpTableView == null)
                        arpTableView = new ARPTableView();

                    contentControlApplication.Content = arpTableView;
                    break;
                case ApplicationViewManager.Name.Lookup:
                    if (lookupView == null)
                        lookupView = new LookupView();

                    contentControlApplication.Content = lookupView;
                    break;
            }

            currentApplicationViewName = name;
        }

        private void ClearSearchFilterOnApplicationListMinimize()
        {
            if (ApplicationView_Expand)
                return;

            if (OpenApplicationList && IsTextBoxSearchFocused)
                return;

            if (OpenApplicationList && IsMouseOverApplicationList)
                return;

            Search = string.Empty;

            // Scroll into view
            listViewApplication.ScrollIntoView(SelectedApplication);
        }
        #endregion

        #region Handle WndProc messages (Single instance, handle HotKeys)
        private HwndSource hwndSoure;

        // This is called after MainWindow() and before OnContentRendered() --> to register hotkeys...
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            hwndSoure = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSoure.AddHook(HwndHook);

            RegisterHotKeys();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Single instance
            if (msg == SingleInstanceHelper.WM_SHOWME)
            {
                ShowWindowAction();
                handled = true;
            }

            // Handle hotkeys
            if (msg == WM_HOTKEY)
            {
                switch (wParam.ToInt32())
                {
                    case 1:
                        ShowWindowAction();
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }
        #endregion

        #region HotKeys (Register / Unregister)
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int WM_HOTKEY = 0x0312;

        /* ID | Command
        *  ---|-------------------
        *  1  | ShowWindowAction()
        */

        List<int> RegisteredHotKeys = new List<int>();

        private void RegisterHotKeys()
        {
            if (SettingsManager.Current.HotKey_ShowWindowEnabled)
            {
                RegisterHotKey(new WindowInteropHelper(this).Handle, 1, SettingsManager.Current.HotKey_ShowWindowModifier, SettingsManager.Current.HotKey_ShowWindowKey);
                RegisteredHotKeys.Add(1);
            }
        }

        private void UnregisterHotKeys()
        {
            // Unregister all registred keys
            foreach (int i in RegisteredHotKeys)
                UnregisterHotKey(new WindowInteropHelper(this).Handle, i);

            // Clear list
            RegisteredHotKeys.Clear();
        }
        #endregion

        #region NotifyIcon
        private void InitNotifyIcon()
        {
            notifyIcon = new NotifyIcon();

            // Get the application icon for the tray
            using (Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Images/NETworkManager.ico")).Stream)
            {
                notifyIcon.Icon = new Icon(iconStream);
            }

            notifyIcon.Text = Title;
            notifyIcon.DoubleClick += new EventHandler(NotifyIcon_DoubleClick);
            notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDown);
            notifyIcon.Visible = SettingsManager.Current.TrayIcon_AlwaysShowIcon;
        }

        private void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Windows.Controls.ContextMenu trayMenu = (System.Windows.Controls.ContextMenu)FindResource("ContextMenuNotifyIcon");
                trayMenu.IsOpen = true;
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowWindowAction();
        }

        private void MetroWindowMain_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (SettingsManager.Current.Window_MinimizeToTrayInsteadOfTaskbar)
                    HideWindowToTray();
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.ContextMenu menu = sender as System.Windows.Controls.ContextMenu;
            menu.DataContext = this;
        }

        private void HideWindowToTray()
        {
            if (notifyIcon == null)
                InitNotifyIcon();

            _isInTray = true;

            notifyIcon.Visible = true;

            Hide();
        }

        private void ShowWindowFromTray()
        {
            _isInTray = false;

            Show();

            notifyIcon.Visible = SettingsManager.Current.TrayIcon_AlwaysShowIcon;
        }

        private void BringWindowToFront()
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            Activate();
        }
        #endregion

        #region ICommands & Actions
        public ICommand OpenWebsiteCommand
        {
            get { return new RelayCommand(p => OpenWebsiteAction(p)); }
        }

        private void OpenWebsiteAction(object url)
        {
            Process.Start((string)url);
        }

        public ICommand OpenApplicationListCommand
        {
            get { return new RelayCommand(p => OpenApplicationListAction()); }
        }

        private void OpenApplicationListAction()
        {
            OpenApplicationList = true;
        }

        public ICommand OpenSettingsCommand
        {
            get { return new RelayCommand(p => OpenSettingsAction()); }
        }

        private void OpenSettingsAction()
        {
            // Save current language code
            if (string.IsNullOrEmpty(_cultureCode))
                _cultureCode = SettingsManager.Current.Localization_CultureCode;

            // Init settings view
            if (_settingsView == null)
            {
                _settingsView = new SettingsView();
                contentControlSettings.Content = _settingsView;
            }

            // Change selected settings view
            _settingsView.SelectedApplicationName = _isInTray ? ApplicationViewManager.Name.None : SelectedApplication.Name;

            // Show the view (this will hide other content)
            ShowSettingsView = true;

            // Bring window to front
            ShowWindowAction();
        }

        public ICommand CloseSettingsCommand
        {
            get { return new RelayCommand(p => CloseSettingsAction()); }
        }

        private async void CloseSettingsAction()
        {
            ShowSettingsView = false;

            // Enable/disable tray icon
            if (!_isInTray)
            {
                if (SettingsManager.Current.TrayIcon_AlwaysShowIcon && notifyIcon == null)
                    InitNotifyIcon();

                if (notifyIcon != null)
                    notifyIcon.Visible = SettingsManager.Current.TrayIcon_AlwaysShowIcon;

                MetroWindowMain.HideOverlay();
            }

            // Ask the user to restart (if he has changed the language)
            if ((_cultureCode != SettingsManager.Current.Localization_CultureCode) || (AllowsTransparency != SettingsManager.Current.Appearance_EnableTransparency))
            {
                ShowWindowAction();

                MetroDialogSettings settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = System.Windows.Application.Current.Resources["String_Button_RestartNow"] as string;
                settings.NegativeButtonText = System.Windows.Application.Current.Resources["String_Button_OK"] as string;
                settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

                ConfigurationManager.Current.FixAirspace = true;

                if (await this.ShowMessageAsync(System.Windows.Application.Current.Resources["String_RestartRequired"] as string, System.Windows.Application.Current.Resources["String_RestartRequiredAfterSettingsChanged"] as string, MessageDialogStyle.AffirmativeAndNegative, settings) == MessageDialogResult.Affirmative)
                {
                    RestartApplication();
                    return;
                }

                ConfigurationManager.Current.FixAirspace = false;
            }

            // Change the transparency
            if ((AllowsTransparency != SettingsManager.Current.Appearance_EnableTransparency) || (Opacity != SettingsManager.Current.Appearance_Opacity))
            {
                if (!AllowsTransparency || !SettingsManager.Current.Appearance_EnableTransparency)
                    Opacity = 1;
                else
                    Opacity = SettingsManager.Current.Appearance_Opacity;
            }

            // Change HotKeys
            if (SettingsManager.HotKeysChanged)
            {
                UnregisterHotKeys();
                RegisterHotKeys();

                SettingsManager.HotKeysChanged = false;
            }

            // Save the settings
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();
        }

        public ICommand ShowWindowCommand
        {
            get { return new RelayCommand(p => ShowWindowAction()); }
        }

        private void ShowWindowAction()
        {
            if (_isInTray)
                ShowWindowFromTray();

            if (!IsActive)
                BringWindowToFront();
        }

        public ICommand CloseApplicationCommand
        {
            get { return new RelayCommand(p => CloseApplicationAction()); }
        }

        private void CloseApplicationAction()
        {
            _closeApplication = true;
            Close();
        }

        private void RestartApplication(bool closeApplication = true)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = ConfigurationManager.Current.ApplicationFullName,
                    Arguments = string.Format("--restart-pid:{0}", Process.GetCurrentProcess().Id)
                }
            }.Start();

            if (closeApplication)
            {
                _closeApplication = true;
                Close();
            }
        }

        public ICommand ApplicationListMouseEnterCommand
        {
            get { return new RelayCommand(p => ApplicationListMouseEnterAction()); }
        }

        private void ApplicationListMouseEnterAction()
        {
            IsMouseOverApplicationList = true;
        }

        public ICommand ApplicationListMouseLeaveCommand
        {
            get { return new RelayCommand(p => ApplicationListMouseLeaveAction()); }
        }

        private void ApplicationListMouseLeaveAction()
        {
            // Don't minmize the list, if the user has accidently mouved the mouse while searching
            if (!IsTextBoxSearchFocused)
                OpenApplicationList = false;

            IsMouseOverApplicationList = false;
        }

        public ICommand TextBoxSearchGotKeyboardFocusCommand
        {
            get { return new RelayCommand(p => TextBoxSearchGotKeyboardFocusAction()); }
        }

        private void TextBoxSearchGotKeyboardFocusAction()
        {
            IsTextBoxSearchFocused = true;
        }

        public ICommand TextBoxSearchLostKeyboardFocusCommand
        {
            get { return new RelayCommand(p => TextBoxSearchLostKeyboardFocusAction()); }
        }

        private void TextBoxSearchLostKeyboardFocusAction()
        {
            if (!IsMouseOverApplicationList)
                OpenApplicationList = false;

            IsTextBoxSearchFocused = false;
        }
        #endregion

        #region Bugfixes
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Window helper
        // Move the window when the user hold the title...
        private void HeaderBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion       
    }
}

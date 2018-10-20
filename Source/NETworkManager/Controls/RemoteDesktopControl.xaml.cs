﻿using NETworkManager.Models.RemoteDesktop;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Threading;
using NETworkManager.Utilities;

namespace NETworkManager.Controls
{
    public partial class RemoteDesktopControl : INotifyPropertyChanged
    {
        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        private bool _initialized;

        private readonly RemoteDesktopSessionInfo _rdpSessionInfo;

        private readonly DispatcherTimer _reconnectAdjustScreenTimer = new DispatcherTimer();

        // Fix WindowsFormsHost width
        private double _rdpClientWidth;
        public double RdpClientWidth
        {
            get => _rdpClientWidth;
            set
            {
                if (value == _rdpClientWidth)
                    return;

                _rdpClientWidth = value;
                OnPropertyChanged();
            }
        }

        // Fix WindowsFormsHost height
        private double _rdpClientHeight;
        public double RdpClientHeight
        {
            get => _rdpClientHeight;
            set
            {
                if (value == _rdpClientHeight)
                    return;

                _rdpClientHeight = value;
                OnPropertyChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (value == _isConnected)
                    return;

                _isConnected = value;
                OnPropertyChanged();
            }
        }

        private bool _isConnecting;
        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if (value == _isConnecting)
                    return;

                _isConnecting = value;
                OnPropertyChanged();
            }
        }

        private string _disconnectReason;
        public string DisconnectReason
        {
            get => _disconnectReason;
            set
            {
                if (value == _disconnectReason)
                    return;

                _disconnectReason = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, load
        public RemoteDesktopControl(RemoteDesktopSessionInfo info)
        {
            InitializeComponent();
            DataContext = this;

            _rdpSessionInfo = info;

            _reconnectAdjustScreenTimer.Tick += ReconnectAdjustScreenTimer_Tick;
            _reconnectAdjustScreenTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Connect after the control is drawn and only on the first init
            if (!_initialized)
            {
                Connect();
                _initialized = true;
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            CloseTab();
        }
        #endregion

        #region ICommands & Actions
        public ICommand ReconnectCommand
        {
            get { return new RelayCommand(p => ReconnectAction()); }
        }

        private void ReconnectAction()
        {
            Reconnect();
        }
        #endregion

        #region Methods
        private void Connect()
        {
            IsConnecting = true;

            RdpClient.CreateControl();

            RdpClient.Server = _rdpSessionInfo.Hostname;
            RdpClient.AdvancedSettings9.RDPPort = _rdpSessionInfo.Port;

            if (_rdpSessionInfo.CustomCredentials)
            {
                RdpClient.UserName = _rdpSessionInfo.Username;
                RdpClient.AdvancedSettings9.ClearTextPassword = SecureStringHelper.ConvertToString(_rdpSessionInfo.Password);
            }

            // AdvancedSettings
            RdpClient.AdvancedSettings9.AuthenticationLevel = _rdpSessionInfo.AuthenticationLevel;
            RdpClient.AdvancedSettings9.EnableCredSspSupport = _rdpSessionInfo.EnableCredSspSupport;

            // Keyboard
            RdpClient.SecuredSettings3.KeyboardHookMode = _rdpSessionInfo.KeyboardHookMode;

            // Devices and resources
            RdpClient.AdvancedSettings9.RedirectClipboard = _rdpSessionInfo.RedirectClipboard;
            RdpClient.AdvancedSettings9.RedirectDevices = _rdpSessionInfo.RedirectDevices;
            RdpClient.AdvancedSettings9.RedirectDrives = _rdpSessionInfo.RedirectDrives;
            RdpClient.AdvancedSettings9.RedirectPorts = _rdpSessionInfo.RedirectPorts;
            RdpClient.AdvancedSettings9.RedirectSmartCards = _rdpSessionInfo.RedirectSmartCards;
            RdpClient.AdvancedSettings9.RedirectPrinters = _rdpSessionInfo.RedirectPrinters;

            // Display
            RdpClient.ColorDepth = _rdpSessionInfo.ColorDepth;      // 8, 15, 16, 24

            if (_rdpSessionInfo.AdjustScreenAutomatically || _rdpSessionInfo.UseCurrentViewSize)
            {
                RdpClient.DesktopWidth = (int)RdpGrid.ActualWidth;
                RdpClient.DesktopHeight = (int)RdpGrid.ActualHeight;
            }
            else
            {
                RdpClient.DesktopWidth = _rdpSessionInfo.DesktopWidth;
                RdpClient.DesktopHeight = _rdpSessionInfo.DesktopHeight;
            }

            FixWindowsFormsHostSize();

            // Events
            RdpClient.OnConnected += RdpClient_OnConnected;
            RdpClient.OnDisconnected += RdpClient_OnDisconnected;

            RdpClient.AdvancedSettings9.EnableWindowsKey = 1;

            RdpClient.Connect();
        }

        private void Reconnect()
        {
            IsConnecting = true;

            if (_rdpSessionInfo.AdjustScreenAutomatically)
            {
                RdpClient.DesktopWidth = (int)RdpGrid.ActualWidth;
                RdpClient.DesktopHeight = (int)RdpGrid.ActualHeight;
            }

            FixWindowsFormsHostSize();

            RdpClient.Connect();
        }

        private void ReconnectAdjustScreen()
        {
            RdpClient.Reconnect((uint)RdpGrid.ActualWidth, (uint)RdpGrid.ActualHeight);
            FixWindowsFormsHostSize();
        }

        private void FixWindowsFormsHostSize()
        {
            RdpClientWidth = RdpClient.DesktopWidth;
            RdpClientHeight = RdpClient.DesktopHeight;
        }

        private void Disconnect()
        {
            if (IsConnected)
                RdpClient.Disconnect();
        }

        public void CloseTab()
        {
            Disconnect();
        }

        private static string GetDisconnectReason(int reason)
        {
            switch (reason)
            {
                case 0:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_NoInfo;
                case 1:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_LocalNotError;
                case 2:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_RemoteByUser;
                case 3:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_ByServer;
                case 260:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_DNSLookupFailed;
                case 262:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_OutOfMemory;
                case 264:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_ConnectionTimedOut;
                case 516:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SocketConnectFailed;
                case 518:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_OutOfMemory2;
                case 520:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_HostNotFound;
                case 772:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_WinsockSendFailed;
                case 774:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_OutOfMemory3;
                case 776:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InvalidIPAddr;
                case 1028:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SocketRecvFailed;
                case 1030:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InvalidSecurityData;
                case 1032:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InternalError;
                case 1286:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InvalidEncryption;
                case 1288:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_DNSLookupFailed2;
                case 1540:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_GetHostByNameFailed;
                case 1542:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InvalidServerSecurityInfo;
                case 1544:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_TimerError;
                case 1796:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_TimeoutOccurred;
                case 1798:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_ServerCertificateUnpackErr;
                case 2052:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InvalidIP;
                case 2055:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrLogonFailure;
                case 2056:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_LicensingFailed;
                case 2308:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_AtClientWinsockFDCLOSE;
                case 2310:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InternalSecurityError;
                case 2312:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_LicensingTimeout;
                case 2566:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_InternalSecurityError2;
                case 2567:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrNoSuchUser;
                case 2822:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_EncryptionError;
                case 2823:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrAccountDisabled;
                case 3078:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_DecryptionError;
                case 3079:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrAccountRestriction;
                case 3080:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_ClientDecompressionError;
                case 3335:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrAccountLockedOut;
                case 3591:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrAccountExpired;
                case 3847:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrPasswordExpired;
                case 4615:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrPasswordMustChange;
                case 5639:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrDelegationPolicy;
                case 5895:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrPolicyNTLMOnly;
                case 6151:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrNoAuthenticatingAuthority;
                case 6919:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrCertExpired;
                case 7175:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrSmartcardWrongPIN;
                case 8455:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrFreshCredRequiredByServer;
                case 8711:
                    return NETworkManager.Resources.Localization.Strings.RemoteDesktopDisconnectReason_SslErrSmartcardCardBlocked;
                default:
                    return "reason not found!";
            }
        }
        #endregion

        #region Events
        private void RdpClient_OnConnected(object sender, EventArgs e)
        {
            IsConnected = true;
            IsConnecting = false;
        }

        private void RdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            IsConnected = false;
            IsConnecting = false;

            DisconnectReason = GetDisconnectReason(e.discReason);
        }

        private void RdpGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Prevent with a timer, that the function (rdpClient.Reconnect()) is executed too often
            if (IsConnected && _rdpSessionInfo.AdjustScreenAutomatically)
                _reconnectAdjustScreenTimer.Start();
        }

        private void ReconnectAdjustScreenTimer_Tick(object sender, EventArgs e)
        {
            // Stop timer
            _reconnectAdjustScreenTimer.Stop();

            // Reconnect with new resulution
            ReconnectAdjustScreen();
        }
        #endregion
    }
}
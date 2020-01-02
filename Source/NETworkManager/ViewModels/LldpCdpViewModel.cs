﻿using System.Collections.Generic;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models.Settings;
using NETworkManager.Models.Network;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using NETworkManager.Utilities;
using System.Windows;
using MahApps.Metro.Controls;
using System.Threading;
using System.Windows.Threading;

namespace NETworkManager.ViewModels
{
    public class LldpCdpViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;

        private DiscoveryProtocol _discoveryProtocol = new DiscoveryProtocol();
        private readonly bool _isLoading;

        private bool _firstRun = true;
        public bool FirstRun
        {
            get => _firstRun;
            set
            {
                if (value == _firstRun)
                    return;

                _firstRun = value;
                OnPropertyChanged();
            }
        }


        private bool _isNetworkInteraceLoading;
        public bool IsNetworkInterfaceLoading
        {
            get => _isNetworkInteraceLoading;
            set
            {
                if (value == _isNetworkInteraceLoading)
                    return;

                _isNetworkInteraceLoading = value;
                OnPropertyChanged();
            }
        }

        private List<NetworkInterfaceInfo> _networkInterfaces;
        public List<NetworkInterfaceInfo> NetworkInterfaces
        {
            get => _networkInterfaces;
            set
            {
                if (value == _networkInterfaces)
                    return;

                _networkInterfaces = value;
                OnPropertyChanged();
            }
        }

        private NetworkInterfaceInfo _selectedNetworkInterface;
        public NetworkInterfaceInfo SelectedNetworkInterface
        {
            get => _selectedNetworkInterface;
            set
            {
                if (value == _selectedNetworkInterface)
                    return;

                if (value != null)
                {
                    if (!_isLoading)
                        SettingsManager.Current.NetworkInterface_SelectedInterfaceId = value.Id;

                    CanCapture = value.IsOperational;
                }

                _selectedNetworkInterface = value;
                OnPropertyChanged();
            }
        }

        private bool _canCapture;
        public bool CanCapture
        {
            get => _canCapture;
            set
            {
                if (value == _canCapture)
                    return;

                _canCapture = value;
                OnPropertyChanged();
            }
        }

        private bool _isCapturing;
        public bool IsCapturing
        {
            get => _isCapturing;
            set
            {
                if (value == _isCapturing)
                    return;

                _isCapturing = value;
                OnPropertyChanged();
            }
        }
      
        /*
        private bool _displayStatusMessage;
        public bool DisplayStatusMessage
        {
            get => _displayStatusMessage;
            set
            {
                if (value == _displayStatusMessage)
                    return;

                _displayStatusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (value == _statusMessage)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
            }
        }
        */

        private DiscoveryProtocolInfo _discoveryInfo;
        public DiscoveryProtocolInfo DiscoveryInfo
        {
            get => _discoveryInfo;
            set
            {
                if (value == _discoveryInfo)
                    return;

                _discoveryInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings, OnShutdown
        public LldpCdpViewModel(IDialogCoordinator instance)
        {
            _isLoading = true;

            _dialogCoordinator = instance;

            LoadNetworkInterfaces();

            // Detect if network address or status changed...
            NetworkChange.NetworkAvailabilityChanged += (sender, args) => ReloadNetworkInterfacesAction();
            NetworkChange.NetworkAddressChanged += (sender, args) => ReloadNetworkInterfacesAction();

            LoadSettings();

            SettingsManager.Current.PropertyChanged += SettingsManager_PropertyChanged;

            _isLoading = false;
        }

        private async void LoadNetworkInterfaces()
        {
            IsNetworkInterfaceLoading = true;

            NetworkInterfaces = await Models.Network.NetworkInterface.GetNetworkInterfacesAsync();

            // Get the last selected interface, if it is still available on this machine...
            if (NetworkInterfaces.Count > 0)
            {
                var info = NetworkInterfaces.FirstOrDefault(s => s.Id == SettingsManager.Current.NetworkInterface_SelectedInterfaceId);

                SelectedNetworkInterface = info ?? NetworkInterfaces[0];
            }

            IsNetworkInterfaceLoading = false;
        }

        private void LoadSettings()
        {

        }
        #endregion

        #region ICommands & Actions
        public ICommand ReloadNetworkInterfacesCommand => new RelayCommand(p => ReloadNetworkInterfacesAction(), ReloadNetworkInterfaces_CanExecute);

        private bool ReloadNetworkInterfaces_CanExecute(object obj) => !IsNetworkInterfaceLoading && Application.Current.MainWindow != null && !((MetroWindow)Application.Current.MainWindow).IsAnyDialogOpen;

        private async void ReloadNetworkInterfacesAction()
        {
            IsNetworkInterfaceLoading = true;

            await Task.Delay(2000); // Make the user happy, let him see a reload animation (and he cannot spam the reload command)

            var id = string.Empty;

            if (SelectedNetworkInterface != null)
                id = SelectedNetworkInterface.Id;

            NetworkInterfaces = await Models.Network.NetworkInterface.GetNetworkInterfacesAsync();

            // Change interface...
            SelectedNetworkInterface = string.IsNullOrEmpty(id) ? NetworkInterfaces.FirstOrDefault() : NetworkInterfaces.FirstOrDefault(x => x.Id == id);

            IsNetworkInterfaceLoading = false;
        }

        public ICommand OpenNetworkConnectionsCommand => new RelayCommand(p => OpenNetworkConnectionsAction());

        public async void OpenNetworkConnectionsAction()
        {
            try
            {
                Process.Start("NCPA.cpl");
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, Resources.Localization.Strings.Error, ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }
        }

        public ICommand RestartAsAdminCommand => new RelayCommand(p => RestartAsAdminAction());

        public async void RestartAsAdminAction()
        {
            try
            {
                (Application.Current.MainWindow as MainWindow).RestartApplication(true, true);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, Resources.Localization.Strings.Error, ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }
        }

        public ICommand CaptureCommand => new RelayCommand(p => CaptureAction());

        public async void CaptureAction()
        {
            if (FirstRun)
                FirstRun = false;

            IsCapturing = true;

            try
            {
                DiscoveryProtocolInfo info = await _discoveryProtocol.GetDiscoveryProtocolAsync("Ethernet", 32, DiscoveryProtocol.Protocol.LLDP_CDP);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, Resources.Localization.Strings.Error, ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }

            IsCapturing = false;
        }
        #endregion

        #region Methods


        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }

        public void OnProfileDialogOpen()
        {

        }

        public void OnProfileDialogClose()
        {

        }
        #endregion

        #region Events
        private void SettingsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
        #endregion
    }
}

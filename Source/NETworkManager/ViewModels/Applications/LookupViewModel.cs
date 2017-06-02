﻿using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System;
using System.Collections.ObjectModel;
using NETworkManager.Models.Settings;
using System.Collections.Generic;
using NETworkManager.Models.Network;
using NETworkManager.Helpers;

namespace NETworkManager.ViewModels.Applications
{
    public class LookupViewModel : ViewModelBase
    {
        #region Variables
        private IDialogCoordinator dialogCoordinator;
        MetroDialogSettings dialogSettings = new MetroDialogSettings();

        private bool _isLoading = true;

        private string _macAddress;
        public string MACAddress
        {
            get { return _macAddress; }
            set
            {
                if (value == _macAddress)
                    return;

                _macAddress = value;
                OnPropertyChanged();
            }
        }

        private List<string> _macAddressHistory = new List<string>();
        public List<string> MACAddressHistory
        {
            get { return _macAddressHistory; }
            set
            {
                if (value == _macAddressHistory)
                    return;

                if (!_isLoading)
                {
                    SettingsManager.Current.OUILookup_MACAddressHistory = value;
                }

                _macAddressHistory = value;
                OnPropertyChanged();
            }
        }

        private bool _isOUILookupRunning;
        public bool IsOUILookupRunning
        {
            get { return _isOUILookupRunning; }
            set
            {
                if (value == _isOUILookupRunning)
                    return;

                _isOUILookupRunning = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<OUIInfo> _ouiLookupResult = new ObservableCollection<OUIInfo>();
        public ObservableCollection<OUIInfo> OUILookupResult
        {
            get { return _ouiLookupResult; }
            set
            {
                if (value == _ouiLookupResult)
                    return;

                _ouiLookupResult = value;
            }
        }

        private string _ports;
        public string Ports
        {
            get { return _ports; }
            set
            {
                if (value == _ports)
                    return;

                _ports = value;
                OnPropertyChanged();
            }
        }

        private bool _isPortLookupRunning;
        public bool IsPortLookupRunning
        {
            get { return _isPortLookupRunning; }
            set
            {
                if (value == _isPortLookupRunning)
                    return;

                _isPortLookupRunning = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<PortInfo> _portLookupResult = new ObservableCollection<PortInfo>();
        public ObservableCollection<PortInfo> PortLookupResult
        {
            get { return _portLookupResult; }
            set
            {
                if (value == _portLookupResult)
                    return;

                _portLookupResult = value;
            }
        }
        #endregion

        #region Constructor, Load settings
        public LookupViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            dialogSettings.CustomResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("NETworkManager;component/Resources/Styles/MetroDialogStyles.xaml", UriKind.RelativeOrAbsolute)
            };

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            if (SettingsManager.Current.Traceroute_HostnameOrIPAddressHistory != null)
                MACAddressHistory = new List<string>(SettingsManager.Current.OUILookup_MACAddressHistory);
        }
        #endregion

        #region Settings

        #endregion

        #region ICommands & Actions
        public ICommand OUILookupCommand
        {
            get { return new RelayCommand(p => OUILookupAction()); }
        }

        private async void OUILookupAction()
        {
            IsOUILookupRunning = true;

            OUILookupResult.Clear();

            foreach (string macAddress in MACAddress.Replace(" ", "").Split(';'))
            {
                foreach (OUIInfo info in await OUILookup.LookupAsync(macAddress))
                {
                    OUILookupResult.Add(info);
                }
            }

            MACAddressHistory = new List<string>(HistoryListHelper.Modify(MACAddressHistory, MACAddress, 5));

            IsOUILookupRunning = false;
        }

        public ICommand PortLookupCommand
        {
            get { return new RelayCommand(p => PortLookupAction()); }
        }

        private async void PortLookupAction()
        {
            IsPortLookupRunning = true;

            PortLookupResult.Clear();

            int[] ports = await PortRangeHelper.ConvertPortRangeToIntArrayAsync(Ports);

            foreach (PortInfo info in await PortLookup.LookupAsync(ports))
            {
                PortLookupResult.Add(info);
            }
            
            IsPortLookupRunning = false;
        }
        #endregion
    }
}
﻿using NETworkManager.Models.Settings;

namespace NETworkManager.ViewModels.Settings
{
    public class SettingsApplicationIPScannerViewModel : ViewModelBase
    {
        #region Variables
        private bool _isLoading = true;

        private int _threads;
        public int Threads
        {
            get { return _threads; }
            set
            {
                if (value == _threads)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.IPScanner_Threads = value;

                _threads = value;
                OnPropertyChanged();
            }
        }

        private int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (value == _timeout)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.IPScanner_Timeout = value;

                _timeout = value;
                OnPropertyChanged();
            }
        }

        private int _buffer;
        public int Buffer
        {
            get { return _buffer; }
            set
            {
                if (value == _buffer)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.IPScanner_Buffer = value;

                _buffer = value;
                OnPropertyChanged();
            }
        }

        private int _Attempts;
        public int Attempts
        {
            get { return _Attempts; }
            set
            {
                if (value == _Attempts)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.IPScanner_Attempts = value;

                _Attempts = value;
                OnPropertyChanged();
            }
        }

        private bool _resolveHostname;
        public bool ResolveHostname
        {
            get { return _resolveHostname; }
            set
            {
                if (value == _resolveHostname)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.IPScanner_ResolveHostname = value;

                _resolveHostname = value;
                OnPropertyChanged();
            }
        }

        private bool _resolveMACAddress;
        public bool ResolveMACAddress
        {
            get { return _resolveMACAddress; }
            set
            {
                if (value == _resolveMACAddress)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.IPScanner_ResolveMACAddress = value;

                _resolveMACAddress = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, load settings
        public SettingsApplicationIPScannerViewModel()
        {
            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            Timeout = SettingsManager.Current.IPScanner_Timeout;
            Buffer = SettingsManager.Current.IPScanner_Buffer;
            Attempts = SettingsManager.Current.IPScanner_Attempts;
            Threads = SettingsManager.Current.IPScanner_Threads;
            ResolveHostname = SettingsManager.Current.IPScanner_ResolveHostname;
            ResolveMACAddress = SettingsManager.Current.IPScanner_ResolveMACAddress;
        }
        #endregion
    }
}

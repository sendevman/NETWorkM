﻿using Lextm.SharpSnmpLib.Messaging;
using NETworkManager.Settings;
using System.Collections.Generic;
using System.Linq;

namespace NETworkManager.ViewModels
{
    public class SNMPSettingsViewModel : ViewModelBase
    {
        #region Variables
        private readonly bool _isLoading;
                
        public List<WalkMode> WalkModes { get; set; }

        private WalkMode _walkMode;
        public WalkMode WalkMode
        {
            get => _walkMode;
            set
            {
                if (value == _walkMode)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.SNMP_WalkMode = value;

                _walkMode = value;
                OnPropertyChanged();
            }
        }

        private int _timeout;
        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value == _timeout)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.SNMP_Timeout = value;

                _timeout = value;
                OnPropertyChanged();
            }
        }

        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                if (value == _port)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.SNMP_Port = value;

                _port = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Contructor, load settings
        public SNMPSettingsViewModel()
        {
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            WalkModes = System.Enum.GetValues(typeof(WalkMode)).Cast<WalkMode>().OrderBy(x => x.ToString()).ToList();
            WalkMode = WalkModes.First(x => x == SettingsManager.Current.SNMP_WalkMode);
            Timeout = SettingsManager.Current.SNMP_Timeout;
            Port = SettingsManager.Current.SNMP_Port;
        }
        #endregion
    }
}
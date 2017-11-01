﻿using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models.Settings;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NETworkManager.ViewModels.Settings
{
    public class SettingsGeneralSettingsViewModel : ViewModelBase
    {
        #region Variables
        private IDialogCoordinator dialogCoordinator;

        private bool _isLoading = true;

        public Action CloseAction { get; set; }

        private string _locationSelectedPath;
        public string LocationSelectedPath
        {
            get { return _locationSelectedPath; }
            set
            {
                if (value == _locationSelectedPath)
                    return;

                _locationSelectedPath = value;
                OnPropertyChanged();
            }
        }

        private bool _movingFiles;
        public bool MovingFiles
        {
            get { return _movingFiles; }
            set
            {
                if (value == _movingFiles)
                    return;

                _movingFiles = value;
                OnPropertyChanged();
            }
        }

        private bool _isPortable;
        public bool IsPortable
        {
            get { return _isPortable; }
            set
            {
                if (value == _isPortable)
                    return;

                if (!_isLoading)
                    MakePortable(value);

                _isPortable = value;
                OnPropertyChanged();
            }
        }

        private bool _makingPortable;
        public bool MakingPortable
        {
            get { return _makingPortable; }
            set
            {
                if (value == _makingPortable)
                    return;

                _makingPortable = value;
                OnPropertyChanged();
            }
        }

        private bool _resetEverything;
        public bool ResetEverything
        {
            get { return _resetEverything; }
            set
            {
                if (value == _resetEverything)
                    return;

                _resetEverything = value;
                OnPropertyChanged();
            }
        }

        private bool _applicationSettingsExists;
        public bool ApplicationSettingsExists
        {
            get { return _applicationSettingsExists; }
            set
            {
                if (value == _applicationSettingsExists)
                    return;

                _applicationSettingsExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetApplicationSettings;
        public bool ResetApplicationSettings
        {
            get { return _resetApplicationSettings; }
            set
            {
                if (value == _resetApplicationSettings)
                    return;

                _resetApplicationSettings = value;
                OnPropertyChanged();
            }
        }

        private bool _networkInterfaceProfilesExists;
        public bool NetworkInterfaceProfilesExists
        {
            get { return _networkInterfaceProfilesExists; }
            set
            {
                if (value == _networkInterfaceProfilesExists)
                    return;

                _networkInterfaceProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetNetworkInterfaceProfiles;
        public bool ResetNetworkInterfaceProfiles
        {
            get { return _resetNetworkInterfaceProfiles; }
            set
            {
                if (value == _resetNetworkInterfaceProfiles)
                    return;

                _resetNetworkInterfaceProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _ipScannerProfilesExists;
        public bool IPScannerProfilesExists
        {
            get { return _ipScannerProfilesExists; }
            set
            {
                if (value == _ipScannerProfilesExists)
                    return;

                _ipScannerProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetIPScannerProfiles;
        public bool ResetIPScannerProfiles
        {
            get { return _resetIPScannerProfiles; }
            set
            {
                if (value == _resetIPScannerProfiles)
                    return;

                _resetIPScannerProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _wakeOnLANClientsExists;
        public bool WakeOnLANClientsExists
        {
            get { return _wakeOnLANClientsExists; }
            set
            {
                if (value == _wakeOnLANClientsExists)
                    return;

                _wakeOnLANClientsExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetWakeOnLANClients;
        public bool ResetWakeOnLANClients
        {
            get { return _resetWakeOnLANClients; }
            set
            {
                if (value == _resetWakeOnLANClients)
                    return;

                _resetWakeOnLANClients = value;
                OnPropertyChanged();
            }
        }

        private bool _portScannerProfilesExists;
        public bool PortScannerProfilesExists
        {
            get { return _portScannerProfilesExists; }
            set
            {
                if (value == _portScannerProfilesExists)
                    return;

                _portScannerProfilesExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetPortScannerProfiles;
        public bool ResetPortScannerProfiles
        {
            get { return _resetPortScannerProfiles; }
            set
            {
                if (value == _resetPortScannerProfiles)
                    return;

                _resetPortScannerProfiles = value;
                OnPropertyChanged();
            }
        }

        private bool _remoteDesktopSessionsExists;
        public bool RemoteDesktopSessionsExists
        {
            get { return _remoteDesktopSessionsExists; }
            set
            {
                if (value == _remoteDesktopSessionsExists)
                    return;

                _remoteDesktopSessionsExists = value;
                OnPropertyChanged();
            }
        }

        private bool _resetRemoteDesktopSessions;
        public bool ResetRemoteDesktopSessions
        {
            get { return _resetRemoteDesktopSessions; }
            set
            {
                if (value == _resetRemoteDesktopSessions)
                    return;

                _resetRemoteDesktopSessions = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsGeneralSettingsViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            LocationSelectedPath = SettingsManager.GetSettingsLocationNotPortable();
            IsPortable = SettingsManager.GetIsPortable();
        }
        #endregion

        #region ICommands & Actions
        public ICommand BrowseFolderCommand
        {
            get { return new RelayCommand(p => BrowseFolderAction()); }
        }

        private void BrowseFolderAction()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (Directory.Exists(LocationSelectedPath))
                dialog.SelectedPath = LocationSelectedPath;

            System.Windows.Forms.DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                LocationSelectedPath = dialog.SelectedPath;
        }

        public ICommand ChangeSettingsCommand
        {
            get { return new RelayCommand(p => ChangeSettingsAction()); }
        }

        private async void ChangeSettingsAction()
        {
            MovingFiles = true;

            // Try moving files (permissions, file is in use...)
            try
            {
                await SettingsManager.MoveSettingsAsync(SettingsManager.GetSettingsLocation(), LocationSelectedPath);

                Properties.Settings.Default.Settings_CustomSettingsLocation = LocationSelectedPath;

                // Show the user some awesome animation to indicate we are working on it :)
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, Application.Current.Resources["String_Header_Error"] as string, ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }

            LocationSelectedPath = string.Empty;
            LocationSelectedPath = Properties.Settings.Default.Settings_CustomSettingsLocation;

            MovingFiles = false;
        }

        public ICommand RestoreDefaultSettingsLocationCommand
        {
            get { return new RelayCommand(p => RestoreDefaultSettingsLocationAction()); }
        }

        private void RestoreDefaultSettingsLocationAction()
        {
            LocationSelectedPath = SettingsManager.GetDefaultSettingsLocation();
        }

        public ICommand ResetSettingsCommand
        {
            get { return new RelayCommand(p => ResetSettingsAction()); }
        }

        public async void ResetSettingsAction()
        {
            MetroDialogSettings settings = AppearanceManager.MetroDialog;

            settings.AffirmativeButtonText = Application.Current.Resources["String_Button_Continue"] as string;
            settings.NegativeButtonText = Application.Current.Resources["String_Button_Cancel"] as string;

            settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

            string message = Application.Current.Resources["String_SelectedSettingsAreReset"] as string;

            if (ResetEverything || ResetApplicationSettings)
            {
                message += Environment.NewLine + Environment.NewLine + string.Format("* {0}", Application.Current.Resources["String_TheSettingsLocationIsNotAffected"] as string);
                message += Environment.NewLine + string.Format("* {0}", Application.Current.Resources["String_ApplicationIsRestartedAfterwards"] as string);
            }

            if (await dialogCoordinator.ShowMessageAsync(this, Application.Current.Resources["String_Header_AreYouSure"] as string, message, MessageDialogStyle.AffirmativeAndNegative, settings) != MessageDialogResult.Affirmative)
                return;

            bool forceRestart = false;

            if (ApplicationSettingsExists && (ResetEverything || ResetApplicationSettings))
            {
                SettingsManager.Reset();
                forceRestart = true;
            }

            if (NetworkInterfaceProfilesExists && (ResetEverything || ResetNetworkInterfaceProfiles))
                NetworkInterfaceProfileManager.Reset();

            if (IPScannerProfilesExists && (ResetEverything || ResetIPScannerProfiles))
                IPScannerProfileManager.Reset();

            if (WakeOnLANClientsExists && (ResetEverything || ResetWakeOnLANClients))
                WakeOnLANClientManager.Reset();

            if (PortScannerProfilesExists && (ResetEverything || ResetPortScannerProfiles))
                PortScannerProfileManager.Reset();

            if (RemoteDesktopSessionsExists && (ResetEverything || ResetRemoteDesktopSessions))
                RemoteDesktopSessionManager.Reset();

            // Restart after reset or show a completed message
            if (forceRestart)
            {
                CloseAction();
            }
            else
            {
                settings.AffirmativeButtonText = Application.Current.Resources["String_Button_OK"] as string;

                await dialogCoordinator.ShowMessageAsync(this, Application.Current.Resources["String_Header_Success"] as string, Application.Current.Resources["String_SettingsSuccessfullyReset"] as string, MessageDialogStyle.Affirmative, settings);
            }
        }
        #endregion

        #region Methods
        private async void MakePortable(bool isPortable)
        {
            MakingPortable = true;

            // Save settings before moving them
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            // Try moving files (permissions, file is in use...)
            try
            {
                await SettingsManager.MakePortableAsync(isPortable);

                Properties.Settings.Default.Settings_CustomSettingsLocation = string.Empty;
                LocationSelectedPath = SettingsManager.GetSettingsLocationNotPortable();

                // Show the user some awesome animation to indicate we are working on it :)
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, Application.Current.Resources["String_Header_Error"] as string, ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }

            MakingPortable = false;
        }

        public void SaveAndCheckSettings()
        {
            // Save everything
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            if (NetworkInterfaceProfileManager.ProfilesChanged)
                NetworkInterfaceProfileManager.Save();

            if (IPScannerProfileManager.ProfilesChanged)
                IPScannerProfileManager.Save();

            if (WakeOnLANClientManager.ClientsChanged)
                WakeOnLANClientManager.Save();

            if (PortScannerProfileManager.ProfilesChanged)
                PortScannerProfileManager.Save();

            if (RemoteDesktopSessionManager.SessionsChanged)
                RemoteDesktopSessionManager.Save();

            // Check if files exist
            ApplicationSettingsExists = File.Exists(SettingsManager.GetSettingsFilePath());
            NetworkInterfaceProfilesExists = File.Exists(NetworkInterfaceProfileManager.GetProfilesFilePath());
            IPScannerProfilesExists = File.Exists(IPScannerProfileManager.GetProfilesFilePath());
            WakeOnLANClientsExists = File.Exists(WakeOnLANClientManager.GetClientsFilePath());
            PortScannerProfilesExists = File.Exists(PortScannerProfileManager.GetProfilesFilePath());
            RemoteDesktopSessionsExists = File.Exists(RemoteDesktopSessionManager.GetSessionsFilePath());
        }
        #endregion
    }
}
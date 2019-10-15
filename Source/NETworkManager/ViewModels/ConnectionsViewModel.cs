﻿using NETworkManager.Models.Network;
using System;
using System.Collections;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using NETworkManager.Utilities;
using NETworkManager.Models.Settings;
using System.Windows.Threading;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models.Export;
using NETworkManager.Views;

namespace NETworkManager.ViewModels
{
    public class ConnectionsViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;

        private readonly bool _isLoading;
        private readonly DispatcherTimer _autoRefreshTimer = new DispatcherTimer();
        private bool _isTimerPaused;

        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (value == _search)
                    return;

                _search = value;

                ConnectionResultsView.Refresh();

                OnPropertyChanged();
            }
        }

        private ObservableCollection<ConnectionInfo> _connectionResults = new ObservableCollection<ConnectionInfo>();
        public ObservableCollection<ConnectionInfo> ConnectionResults
        {
            get => _connectionResults;
            set
            {
                if (value == _connectionResults)
                    return;

                _connectionResults = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ConnectionResultsView { get; }

        private ConnectionInfo _selectedConnectionInfo;
        public ConnectionInfo SelectedConnectionInfo
        {
            get => _selectedConnectionInfo;
            set
            {
                if (value == _selectedConnectionInfo)
                    return;

                _selectedConnectionInfo = value;
                OnPropertyChanged();
            }
        }

        private IList _selectedConnectionInfos = new ArrayList();
        public IList SelectedConnectionInfos
        {
            get => _selectedConnectionInfos;
            set
            {
                if (Equals(value, _selectedConnectionInfos))
                    return;

                _selectedConnectionInfos = value;
                OnPropertyChanged();
            }
        }

        private bool _autoRefresh;
        public bool AutoRefresh
        {
            get => _autoRefresh;
            set
            {
                if (value == _autoRefresh)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.Connections_AutoRefresh = value;

                _autoRefresh = value;

                // Start timer to refresh automatically
                if (!_isLoading)
                {
                    if (value)
                        StartAutoRefreshTimer();
                    else
                        StopAutoRefreshTimer();
                }

                OnPropertyChanged();
            }
        }

        public ICollectionView AutoRefreshTimes { get; }

        private AutoRefreshTimeInfo _selectedAutoRefreshTime;
        public AutoRefreshTimeInfo SelectedAutoRefreshTime
        {
            get => _selectedAutoRefreshTime;
            set
            {
                if (value == _selectedAutoRefreshTime)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.Connections_AutoRefreshTime = value;

                _selectedAutoRefreshTime = value;

                if (AutoRefresh)
                    ChangeAutoRefreshTimerInterval(AutoRefreshTime.CalculateTimeSpan(value));

                OnPropertyChanged();
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (value == _isRefreshing)
                    return;

                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

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
        #endregion

        #region Contructor, load settings
        public ConnectionsViewModel(IDialogCoordinator instance)
        {
            _isLoading = true;

            _dialogCoordinator = instance;

            // Result view + search
            ConnectionResultsView = CollectionViewSource.GetDefaultView(ConnectionResults);
            ConnectionResultsView.SortDescriptions.Add(new SortDescription(nameof(ConnectionInfo.LocalIPAddressInt32), ListSortDirection.Ascending));
            ConnectionResultsView.Filter = o =>
            {
                if (string.IsNullOrEmpty(Search))
                    return true;

                // Search by local/remote IP Address, local/remote Port, Protocol and State
                return o is ConnectionInfo info && (info.LocalIPAddress.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1 || info.LocalPort.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1 || info.RemoteIPAddress.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1 || info.RemotePort.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1 || info.Protocol.ToString().IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1 || Resources.Localization.Strings.ResourceManager.GetString("TcpState_" + info.TcpState.ToString(), LocalizationManager.Culture).IndexOf(Search, StringComparison.OrdinalIgnoreCase) > -1);
            };

            AutoRefreshTimes = CollectionViewSource.GetDefaultView(AutoRefreshTime.Defaults);
            SelectedAutoRefreshTime = AutoRefreshTimes.SourceCollection.Cast<AutoRefreshTimeInfo>().FirstOrDefault(x => (x.Value == SettingsManager.Current.Connections_AutoRefreshTime.Value && x.TimeUnit == SettingsManager.Current.Connections_AutoRefreshTime.TimeUnit));

            _autoRefreshTimer.Tick += AutoRefreshTimer_Tick;

            LoadSettings();

            _isLoading = false;

            Refresh();

            if (AutoRefresh)
                StartAutoRefreshTimer();
        }

        private void LoadSettings()
        {
            AutoRefresh = SettingsManager.Current.Connections_AutoRefresh;
        }
        #endregion

        #region ICommands & Actions
        public ICommand RefreshCommand => new RelayCommand(p => RefreshAction(), Refresh_CanExecute);

        private bool Refresh_CanExecute(object paramter)
        {
            return Application.Current.MainWindow != null && !((MetroWindow)Application.Current.MainWindow).IsAnyDialogOpen;
        }

        private void RefreshAction()
        {
            DisplayStatusMessage = false;

            Refresh();
        }

        public ICommand CopySelectedLocalIpAddressCommand => new RelayCommand(p => CopySelectedLocalIpAddressAction());

        private void CopySelectedLocalIpAddressAction()
        {
            CommonMethods.SetClipboard(SelectedConnectionInfo.LocalIPAddress.ToString());
        }

        public ICommand CopySelectedLocalPortCommand => new RelayCommand(p => CopySelectedLocalPortAction());

        private void CopySelectedLocalPortAction()
        {
            CommonMethods.SetClipboard(SelectedConnectionInfo.LocalPort.ToString());
        }

        public ICommand CopySelectedRemoteIpAddressCommand => new RelayCommand(p => CopySelectedRemoteIpAddressAction());

        private void CopySelectedRemoteIpAddressAction()
        {
            CommonMethods.SetClipboard(SelectedConnectionInfo.RemoteIPAddress.ToString());
        }

        public ICommand CopySelectedRemotePortCommand => new RelayCommand(p => CopySelectedRemotePortAction());

        private void CopySelectedRemotePortAction()
        {
            CommonMethods.SetClipboard(SelectedConnectionInfo.RemotePort.ToString());
        }

        public ICommand CopySelectedProtocolCommand => new RelayCommand(p => CopySelectedProtocolAction());

        private void CopySelectedProtocolAction()
        {
            CommonMethods.SetClipboard(SelectedConnectionInfo.Protocol.ToString());
        }

        public ICommand CopySelectedStateCommand => new RelayCommand(p => CopySelectedStateAction());

        private void CopySelectedStateAction()
        {
            CommonMethods.SetClipboard(Resources.Localization.Strings.ResourceManager.GetString("TcpState_" + SelectedConnectionInfo.TcpState, LocalizationManager.Culture));
        }

        public ICommand ExportCommand => new RelayCommand(p => ExportAction());

        private async void ExportAction()
        {
            var customDialog = new CustomDialog
            {
                Title = Resources.Localization.Strings.Export
            };

            var exportViewModel = new ExportViewModel(async instance =>
            {
                await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                try
                {
                    ExportManager.Export(instance.FilePath, instance.FileType, instance.ExportAll ? ConnectionResults : new ObservableCollection<ConnectionInfo>(SelectedConnectionInfos.Cast<ConnectionInfo>().ToArray()));
                }
                catch (Exception ex)
                {
                    var settings = AppearanceManager.MetroDialog;
                    settings.AffirmativeButtonText = Resources.Localization.Strings.OK;

                    await _dialogCoordinator.ShowMessageAsync(this, Resources.Localization.Strings.Error, Resources.Localization.Strings.AnErrorOccurredWhileExportingTheData + Environment.NewLine + Environment.NewLine + ex.Message, MessageDialogStyle.Affirmative, settings);
                }

                SettingsManager.Current.Connections_ExportFileType = instance.FileType;
                SettingsManager.Current.Connections_ExportFilePath = instance.FilePath;
            }, instance => { _dialogCoordinator.HideMetroDialogAsync(this, customDialog); }, SettingsManager.Current.Connections_ExportFileType, SettingsManager.Current.Connections_ExportFilePath);

            customDialog.Content = new ExportDialog
            {
                DataContext = exportViewModel
            };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        #endregion

        #region Methods
        private async void Refresh()
        {
            IsRefreshing = true;

            ConnectionResults.Clear();

            (await Connection.GetActiveTcpConnectionsAsync()).ForEach(x => ConnectionResults.Add(x));
            
            IsRefreshing = false;
        }         

        private void ChangeAutoRefreshTimerInterval(TimeSpan timeSpan)
        {
            _autoRefreshTimer.Interval = timeSpan;
        }

        private void StartAutoRefreshTimer()
        {
            ChangeAutoRefreshTimerInterval(AutoRefreshTime.CalculateTimeSpan(SelectedAutoRefreshTime));

            _autoRefreshTimer.Start();
        }

        private void StopAutoRefreshTimer()
        {
            _autoRefreshTimer.Stop();
        }

        private void PauseAutoRefreshTimer()
        {
            if (!_autoRefreshTimer.IsEnabled)
                return;

            _autoRefreshTimer.Stop();
            _isTimerPaused = true;
        }

        private void ResumeAutoRefreshTimer()
        {
            if (!_isTimerPaused)
                return;

            _autoRefreshTimer.Start();
            _isTimerPaused = false;
        }
                
        public void OnViewVisible()
        {
            ResumeAutoRefreshTimer();
        }

        public void OnViewHide()
        {
            PauseAutoRefreshTimer();
        }

        #endregion

        #region Events
        private void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            // Stop timer...
            _autoRefreshTimer.Stop();

            // Refresh
            Refresh();

            // Restart timer...
            _autoRefreshTimer.Start();
        }
        #endregion
    }
}
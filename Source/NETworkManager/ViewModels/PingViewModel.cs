﻿using NETworkManager.Models.Network;
using NETworkManager.Models.Settings;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Data;
using Dragablz;
using NETworkManager.Controls;
using NETworkManager.Utilities;
using System.Collections.ObjectModel;
using System.Globalization;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models.Export;
using NETworkManager.Views;

namespace NETworkManager.ViewModels
{
    public class PingViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly int _tabId;
        private bool _firstLoad = true;

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly bool _isLoading;

        private string _host;
        public string Host
        {
            get => _host;
            set
            {
                if (value == _host)
                    return;

                _host = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView HostHistoryView { get; }

        private bool _isPingRunning;
        public bool IsPingRunning
        {
            get => _isPingRunning;
            set
            {
                if (value == _isPingRunning)
                    return;

                _isPingRunning = value;
                OnPropertyChanged();
            }
        }

        private bool _cancelPing;
        public bool CancelPing
        {
            get => _cancelPing;
            set
            {
                if (value == _cancelPing)
                    return;

                _cancelPing = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PingInfo> _pingResults = new ObservableCollection<PingInfo>();
        public ObservableCollection<PingInfo> PingResults
        {
            get => _pingResults;
            set
            {
                if (value != null && value == _pingResults)
                    return;

                _pingResults = value;
            }
        }

        public ICollectionView PingResultsView { get; }

        private PingInfo _selectedPingResult;
        public PingInfo SelectedPingResult
        {
            get => _selectedPingResult;
            set
            {
                if (value == _selectedPingResult)
                    return;

                _selectedPingResult = value;
                OnPropertyChanged();
            }
        }

        private IList _selectedPingResults = new ArrayList();
        public IList SelectedPingResults
        {
            get => _selectedPingResults;
            set
            {
                if (Equals(value, _selectedPingResults))
                    return;

                _selectedPingResults = value;
                OnPropertyChanged();
            }
        }

        private int _pingsTransmitted;
        public int PingsTransmitted
        {
            get => _pingsTransmitted;
            set
            {
                if (value == _pingsTransmitted)
                    return;

                _pingsTransmitted = value;
                OnPropertyChanged();
            }
        }

        private int _pingsReceived;
        public int PingsReceived
        {
            get => _pingsReceived;
            set
            {
                if (value == _pingsReceived)
                    return;

                _pingsReceived = value;
                OnPropertyChanged();
            }
        }

        private int _pingsLost;
        public int PingsLost
        {
            get => _pingsLost;
            set
            {
                if (value == _pingsLost)
                    return;

                _pingsLost = value;
                OnPropertyChanged();
            }
        }

        private long _minimumTime;
        public long MinimumTime
        {
            get => _minimumTime;
            set
            {
                if (value == _minimumTime)
                    return;

                _minimumTime = value;
                OnPropertyChanged();
            }
        }

        private long _maximumTime;
        public long MaximumTime
        {
            get => _maximumTime;
            set
            {
                if (value == _maximumTime)
                    return;

                _maximumTime = value;
                OnPropertyChanged();
            }
        }

        private int _averageTime;
        public int AverageTime
        {
            get => _averageTime;
            set
            {
                if (value == _averageTime)
                    return;

                _averageTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get => _startTime;
            set
            {
                if (value == _startTime)
                    return;

                _startTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (value == _duration)
                    return;

                _duration = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _endTime;
        public DateTime? EndTime
        {
            get => _endTime;
            set
            {
                if (value == _endTime)
                    return;

                _endTime = value;
                OnPropertyChanged();
            }
        }

        private bool _expandStatistics;
        public bool ExpandStatistics
        {
            get => _expandStatistics;
            set
            {
                if (value == _expandStatistics)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.Ping_ExpandStatistics = value;

                _expandStatistics = value;
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

        public bool ShowStatistics => SettingsManager.Current.Ping_ShowStatistics;

        public bool HighlightTimeouts => SettingsManager.Current.Ping_HighlightTimeouts;
        #endregion

        #region Contructor, load settings    
        public PingViewModel(IDialogCoordinator instance, int tabId, string host)
        {
            _isLoading = true;

            _dialogCoordinator = instance;

            _tabId = tabId;
            Host = host;

            // Set collection view
            HostHistoryView = CollectionViewSource.GetDefaultView(SettingsManager.Current.Ping_HostHistory);

            // Result view
            PingResultsView = CollectionViewSource.GetDefaultView(PingResults);

            LoadSettings();

            // Detect if settings have changed...
            SettingsManager.Current.PropertyChanged += SettingsManager_PropertyChanged;

            _isLoading = false;
        }

        public void OnLoaded()
        {
            if (!_firstLoad)
                return;

            if (!string.IsNullOrEmpty(Host))
                StartPing();

            _firstLoad = false;
        }

        private void LoadSettings()
        {
            ExpandStatistics = SettingsManager.Current.Ping_ExpandStatistics;
        }
        #endregion

        #region ICommands & Actions
        public ICommand PingCommand
        {
            get { return new RelayCommand(p => PingAction()); }
        }

        private void PingAction()
        {
            if (IsPingRunning)
                StopPing();
            else
                StartPing();
        }

        public ICommand CopySelectedTimestampCommand
        {
            get { return new RelayCommand(p => CopySelectedTimestampAction()); }
        }

        private void CopySelectedTimestampAction()
        {
            CommonMethods.SetClipboard(SelectedPingResult.Timestamp.ToString(CultureInfo.CurrentCulture));
        }

        public ICommand CopySelectedIPAddressCommand
        {
            get { return new RelayCommand(p => CopySelectedIPAddressAction()); }
        }

        private void CopySelectedIPAddressAction()
        {
            CommonMethods.SetClipboard(SelectedPingResult.IPAddress.ToString());
        }

        public ICommand CopySelectedHostnameCommand
        {
            get { return new RelayCommand(p => CopySelectedHostnameAction()); }
        }

        private void CopySelectedHostnameAction()
        {
            CommonMethods.SetClipboard(SelectedPingResult.Hostname);
        }

        public ICommand CopySelectedBytesCommand
        {
            get { return new RelayCommand(p => CopySelectedBytesAction()); }
        }

        private void CopySelectedBytesAction()
        {
            CommonMethods.SetClipboard(SelectedPingResult.Bytes.ToString());
        }

        public ICommand CopySelectedTimeCommand
        {
            get { return new RelayCommand(p => CopySelectedTimeAction()); }
        }

        private void CopySelectedTimeAction()
        {
            CommonMethods.SetClipboard(SelectedPingResult.Time.ToString());
        }

        public ICommand CopySelectedTTLCommand
        {
            get { return new RelayCommand(p => CopySelectedTTLAction()); }
        }

        private void CopySelectedTTLAction()
        {
            CommonMethods.SetClipboard(SelectedPingResult.TTL.ToString());
        }

        public ICommand CopySelectedStatusCommand
        {
            get { return new RelayCommand(p => CopySelectedStatusAction()); }
        }

        private void CopySelectedStatusAction()
        {
            CommonMethods.SetClipboard(Resources.Localization.Strings.ResourceManager.GetString("IPStatus_" + SelectedPingResult.Status, LocalizationManager.Culture));
        }

        public ICommand ExportCommand
        {
            get { return new RelayCommand(p => ExportAction()); }
        }

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
                    ExportManager.Export(instance.FilePath, instance.FileType, instance.ExportAll ? PingResults : new ObservableCollection<PingInfo>(SelectedPingResults.Cast<PingInfo>().ToArray()));
                }
                catch (Exception ex)
                {
                    var settings = AppearanceManager.MetroDialog;
                    settings.AffirmativeButtonText = Resources.Localization.Strings.OK;

                    await _dialogCoordinator.ShowMessageAsync(this, Resources.Localization.Strings.Error, Resources.Localization.Strings.AnErrorOccurredWhileExportingTheData + Environment.NewLine + Environment.NewLine + ex.Message, MessageDialogStyle.Affirmative, settings);
                }

                SettingsManager.Current.Ping_ExportFileType = instance.FileType;
                SettingsManager.Current.Ping_ExportFilePath = instance.FilePath;
            }, instance => { _dialogCoordinator.HideMetroDialogAsync(this, customDialog); }, SettingsManager.Current.Ping_ExportFileType, SettingsManager.Current.Ping_ExportFilePath);

            customDialog.Content = new ExportDialog
            {
                DataContext = exportViewModel
            };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        #endregion

        #region Methods      
        private async void StartPing()
        {
            DisplayStatusMessage = false;
            IsPingRunning = true;

            // Measure the time
            StartTime = DateTime.Now;
            _stopwatch.Start();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _dispatcherTimer.Start();
            EndTime = null;

            // Reset the latest results
            PingResults.Clear();
            PingsTransmitted = 0;
            PingsReceived = 0;
            PingsLost = 0;
            AverageTime = 0;
            MinimumTime = 0;
            MaximumTime = 0;

            // Change the tab title (not nice, but it works)
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);

            if (window != null)
            {
                foreach (var tabablzControl in VisualTreeHelper.FindVisualChildren<TabablzControl>(window))
                {
                    tabablzControl.Items.OfType<DragablzTabItem>().First(x => x.Id == _tabId).Header = Host;
                }
            }

            // Try to parse the string into an IP-Address
            var hostIsIP = IPAddress.TryParse(Host, out var ipAddress);

            if (!hostIsIP)
            {
                try
                {
                    // Try to resolve the hostname
                    var ipHostEntrys = await Dns.GetHostEntryAsync(Host);


                    foreach (var ip in ipHostEntrys.AddressList)
                    {
                        switch (ip.AddressFamily)
                        {
                            case AddressFamily.InterNetwork when SettingsManager.Current.Ping_ResolveHostnamePreferIPv4:
                                ipAddress = ip;
                                break;
                            case AddressFamily.InterNetworkV6 when !SettingsManager.Current.Ping_ResolveHostnamePreferIPv4:
                                ipAddress = ip;
                                break;
                        }
                    }

                    // Fallback --> If we could not resolve our prefered ip protocol for the hostname
                    foreach (var ip in ipHostEntrys.AddressList)
                    {
                        ipAddress = ip;
                        break;
                    }
                }
                catch (SocketException) // This will catch DNS resolve errors
                {
                    if (CancelPing)
                        UserHasCanceled();
                    else
                        PingFinished();

                    StatusMessage = string.Format(Resources.Localization.Strings.CouldNotResolveHostnameFor, Host);
                    DisplayStatusMessage = true;

                    return;
                }
            }

            // Add the hostname or ip address to the history
            AddHostToHistory(Host);

            _cancellationTokenSource = new CancellationTokenSource();


            var ping = new Ping
            {
                Attempts = SettingsManager.Current.Ping_Attempts,
                Timeout = SettingsManager.Current.Ping_Timeout,
                Buffer = new byte[SettingsManager.Current.Ping_Buffer],
                TTL = SettingsManager.Current.Ping_TTL,
                DontFragment = SettingsManager.Current.Ping_DontFragment,
                WaitTime = SettingsManager.Current.Ping_WaitTime,
                ExceptionCancelCount = SettingsManager.Current.Ping_ExceptionCancelCount,
                Hostname = hostIsIP ? string.Empty : Host
            };

            ping.PingReceived += Ping_PingReceived;
            ping.PingCompleted += Ping_PingCompleted;
            ping.PingException += Ping_PingException;
            ping.UserHasCanceled += Ping_UserHasCanceled;

            ping.SendAsync(ipAddress, _cancellationTokenSource.Token);
        }

        private void StopPing()
        {
            CancelPing = true;

            _cancellationTokenSource?.Cancel();
        }

        private void UserHasCanceled()
        {
            CancelPing = false;

            PingFinished();
        }

        private void PingFinished()
        {
            IsPingRunning = false;

            // Stop timer and stopwatch
            _stopwatch.Stop();
            _dispatcherTimer.Stop();

            Duration = _stopwatch.Elapsed;
            EndTime = DateTime.Now;

            _stopwatch.Reset();
        }

        private void AddHostToHistory(string host)
        {
            // Create the new list
            var list = ListHelper.Modify(SettingsManager.Current.Ping_HostHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.Ping_HostHistory.Clear();
            OnPropertyChanged(nameof(Host)); // Raise property changed again, after the collection has been cleared

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.Ping_HostHistory.Add(x));
        }

        public void OnClose()
        {
            // Stop the ping
            if (IsPingRunning)
                PingAction();
        }
        #endregion

        #region Events
        private void Ping_PingReceived(object sender, PingReceivedArgs e)
        {
            var pingInfo = PingInfo.Parse(e);

            // Add the result to the collection
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                lock (PingResults)
                    PingResults.Add(pingInfo);
            }));

            // Calculate statistics
            PingsTransmitted++;

            if (pingInfo.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                PingsReceived++;

                if (PingsReceived == 1)
                {
                    MinimumTime = pingInfo.Time;
                    MaximumTime = pingInfo.Time;
                }
                else
                {
                    if (MinimumTime > pingInfo.Time)
                        MinimumTime = pingInfo.Time;

                    if (MaximumTime < pingInfo.Time)
                        MaximumTime = pingInfo.Time;

                    // lock, because the collection is changed from another thread...
                    // I hope this won't slow the application or causes a hight cpu load
                    lock (PingResults)
                        AverageTime = (int)PingResults.Average(s => s.Time);
                }
            }
            else
            {
                PingsLost++;
            }
        }

        private void Ping_PingCompleted(object sender, EventArgs e)
        {
            PingFinished();
        }

        private void Ping_PingException(object sender, PingExceptionArgs e)
        {
            // Get the error code and change the message (maybe we can help the user with troubleshooting)

            var errorMessage = string.Empty;

            if (e.InnerException is Win32Exception w32Ex)
            {
                switch (w32Ex.NativeErrorCode)
                {
                    case 1231:
                        errorMessage = Resources.Localization.Strings.NetworkLocationCannotBeReachedMessage;
                        break;
                    default:
                        errorMessage = e.InnerException.Message;
                        break;
                }
            }

            PingFinished();

            StatusMessage = errorMessage;
            DisplayStatusMessage = true;
        }

        private void Ping_UserHasCanceled(object sender, EventArgs e)
        {
            UserHasCanceled();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Duration = _stopwatch.Elapsed;
        }

        private void SettingsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsInfo.Ping_ShowStatistics))
                OnPropertyChanged(nameof(ShowStatistics));

            if (e.PropertyName == nameof(SettingsInfo.Ping_HighlightTimeouts))
                OnPropertyChanged(nameof(HighlightTimeouts));
        }
        #endregion
    }
}
﻿using NETworkManager.Settings;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System;
using NETworkManager.Models.Network;
using System.ComponentModel;
using System.Windows.Data;
using NETworkManager.Utilities;
using System.Linq;
using System.Collections.ObjectModel;
using NETworkManager.Views;
using System.Net;
using NETworkManager.Profiles;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NETworkManager.Models;

namespace NETworkManager.ViewModels
{
    public class PingMonitorHostViewModel : ViewModelBase, IProfileManager
    {
        #region  Variables 
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly DispatcherTimer _searchDispatcherTimer = new DispatcherTimer();

        private readonly bool _isLoading = true;
        private bool _isViewActive = true;

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

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (value == _isRunning)
                    return;

                _isRunning = value;
                OnPropertyChanged();
            }
        }

        private bool _isStatusMessageDisplayed;
        public bool IsStatusMessageDisplayed
        {
            get => _isStatusMessageDisplayed;
            set
            {
                if (value == _isStatusMessageDisplayed)
                    return;

                _isStatusMessageDisplayed = value;
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

        private ObservableCollection<PingMonitorView> _hosts = new();
        public ObservableCollection<PingMonitorView> Hosts
        {
            get => _hosts;
            set
            {
                if (value != null && value == _hosts)
                    return;

                _hosts = value;
            }
        }

        public ICollectionView HostsView { get; }

        private PingMonitorView _selectedHost;
        public PingMonitorView SelectedHost
        {
            get => _selectedHost;
            set
            {
                if (value == _selectedHost)
                    return;

                _selectedHost = value;
                OnPropertyChanged();
            }
        }

        #region Profiles
        public ICollectionView Profiles { get; }

        private ProfileInfo _selectedProfile;
        public ProfileInfo SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (value == _selectedProfile)
                    return;

                _selectedProfile = value;
                OnPropertyChanged();
            }
        }

        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (value == _search)
                    return;

                _search = value;

                StartDelayedSearch();

                OnPropertyChanged();
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                if (value == _isSearching)
                    return;

                _isSearching = value;
                OnPropertyChanged();
            }
        }

        private bool _canProfileWidthChange = true;
        private double _tempProfileWidth;

        private bool _expandProfileView;
        public bool ExpandProfileView
        {
            get => _expandProfileView;
            set
            {
                if (value == _expandProfileView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.PingMonitor_ExpandProfileView = value;

                _expandProfileView = value;

                if (_canProfileWidthChange)
                    ResizeProfile(false);

                OnPropertyChanged();
            }
        }

        private GridLength _profileWidth;
        public GridLength ProfileWidth
        {
            get => _profileWidth;
            set
            {
                if (value == _profileWidth)
                    return;

                if (!_isLoading && Math.Abs(value.Value - GlobalStaticConfiguration.Profile_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix) // Do not save the size when collapsed
                    SettingsManager.Current.PingMonitor_ProfileWidth = value.Value;

                _profileWidth = value;

                if (_canProfileWidthChange)
                    ResizeProfile(true);

                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Constructor, load settings
        public PingMonitorHostViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;

            // Host history
            HostHistoryView = CollectionViewSource.GetDefaultView(SettingsManager.Current.PingMonitor_HostHistory);

            // Hosts
            HostsView = CollectionViewSource.GetDefaultView(Hosts);

            // Profiles
            Profiles = new CollectionViewSource { Source = ProfileManager.Groups.SelectMany(x => x.Profiles) }.View;
            Profiles.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ProfileInfo.Group)));
            Profiles.SortDescriptions.Add(new SortDescription(nameof(ProfileInfo.Group), ListSortDirection.Ascending));
            Profiles.SortDescriptions.Add(new SortDescription(nameof(ProfileInfo.Name), ListSortDirection.Ascending));
            Profiles.Filter = o =>
            {
                if (!(o is ProfileInfo info))
                    return false;

                if (string.IsNullOrEmpty(Search))
                    return info.PingMonitor_Enabled;

                var search = Search.Trim();

                // Search by: Tag=xxx (exact match, ignore case)
                /*
                if (search.StartsWith(ProfileManager.TagIdentifier, StringComparison.OrdinalIgnoreCase))
                    return !string.IsNullOrEmpty(info.Tags) && info.PingMonitor_Enabled && info.Tags.Replace(" ", "").Split(';').Any(str => search.Substring(ProfileManager.TagIdentifier.Length, search.Length - ProfileManager.TagIdentifier.Length).Equals(str, StringComparison.OrdinalIgnoreCase));
                */

                // Search by: Name, Ping_Host
                return info.PingMonitor_Enabled && (info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || info.PingMonitor_Host.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1);
            };

            // This will select the first entry as selected item...
            SelectedProfile = Profiles.SourceCollection.Cast<ProfileInfo>().Where(x => x.PingMonitor_Enabled).OrderBy(x => x.Group).ThenBy(x => x.Name).FirstOrDefault();

            ProfileManager.OnProfilesUpdated += ProfileManager_OnProfilesUpdated;

            _searchDispatcherTimer.Interval = GlobalStaticConfiguration.SearchDispatcherTimerTimeSpan;
            _searchDispatcherTimer.Tick += SearchDispatcherTimer_Tick;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            ExpandProfileView = SettingsManager.Current.PingMonitor_ExpandProfileView;

            ProfileWidth = ExpandProfileView ? new GridLength(SettingsManager.Current.PingMonitor_ProfileWidth) : new GridLength(GlobalStaticConfiguration.Profile_WidthCollapsed);

            _tempProfileWidth = SettingsManager.Current.PingMonitor_ProfileWidth;
        }
        #endregion

        #region ICommands & Actions
        public ICommand AddHostCommand => new RelayCommand(p => AddHostAction());

        private void AddHostAction()
        {
            AddHost(Host);

            // Add the hostname or ip address to the history
            AddHostToHistory(Host);

            Host = "";
        }

        public ICommand ExportCommand => new RelayCommand(p => ExportAction());

        private void ExportAction()
        {
            if (SelectedHost != null)
                SelectedHost.Export();
        }

        public ICommand AddHostProfileCommand => new RelayCommand(p => AddHostProfileAction(), AddHostProfile_CanExecute);

        private bool AddHostProfile_CanExecute(object obj)
        {
            return !IsSearching && SelectedProfile != null;
        }

        private void AddHostProfileAction()
        {
            AddHost(SelectedProfile.PingMonitor_Host);
        }

        public ICommand AddProfileCommand => new RelayCommand(p => AddProfileAction());

        private void AddProfileAction()
        {
            ProfileDialogManager.ShowAddProfileDialog(this, _dialogCoordinator, null, ApplicationName.PingMonitor);
        }

        private bool ModifyProfile_CanExecute(object obj) => SelectedProfile != null && !SelectedProfile.IsDynamic;

        public ICommand EditProfileCommand => new RelayCommand(p => EditProfileAction(), ModifyProfile_CanExecute);

        private void EditProfileAction()
        {
            ProfileDialogManager.ShowEditProfileDialog(this, _dialogCoordinator, SelectedProfile);
        }

        public ICommand CopyAsProfileCommand => new RelayCommand(p => CopyAsProfileAction(), ModifyProfile_CanExecute);

        private void CopyAsProfileAction()
        {
            ProfileDialogManager.ShowCopyAsProfileDialog(this, _dialogCoordinator, SelectedProfile);
        }

        public ICommand DeleteProfileCommand => new RelayCommand(p => DeleteProfileAction(), ModifyProfile_CanExecute);

        private void DeleteProfileAction()
        {
            ProfileDialogManager.ShowDeleteProfileDialog(this, _dialogCoordinator, new List<ProfileInfo> { SelectedProfile });
        }

        public ICommand EditGroupCommand => new RelayCommand(EditGroupAction);

        private void EditGroupAction(object group)
        {
            ProfileDialogManager.ShowEditGroupDialog(this, _dialogCoordinator, ProfileManager.GetGroup(group.ToString()));
        }

        public ICommand ClearSearchCommand => new RelayCommand(p => ClearSearchAction());

        private void ClearSearchAction()
        {
            Search = string.Empty;
        }
        #endregion

        #region Methods
        public void AddHost(string hosts)
        {
            IsStatusMessageDisplayed = false;
            StatusMessage = string.Empty;

            IsRunning = true;

            Task.Run(() =>
            {
                Parallel.ForEach(hosts.Split(';'), currentHost =>
                {
                    var host = currentHost.Trim();
                    string hostname = string.Empty;

                    // Resolve ip address from hostname
                    if (!IPAddress.TryParse(host, out var ipAddress))
                    {
                        hostname = host;

                        using var dnsResolverTask = DNSClientHelper.ResolveAorAaaaAsync(host, SettingsManager.Current.Network_ResolveHostnamePreferIPv4);

                        // Wait for task inside a Parallel.Foreach
                        dnsResolverTask.Wait();

                        if (dnsResolverTask.Result.HasError)
                        {
                            StatusMessageShowOrAdd(host, dnsResolverTask.Result);
                            return;
                        }

                        ipAddress = dnsResolverTask.Result.Value;
                    }

                    // Resolve hostname from ip address
                    else
                    {
                        using var dnsResolverTask = DNSClient.GetInstance().ResolvePtrAsync(ipAddress);

                        // Wait for task inside a Parallel.Foreach
                        dnsResolverTask.Wait();

                        // Hostname is not necessary for ping. Don't show an error message in the UI.
                        if (!dnsResolverTask.Result.HasError)
                            hostname = dnsResolverTask.Result.Value;
                    }

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        Hosts.Add(new PingMonitorView(Guid.NewGuid(), RemoveHost, new PingMonitorOptions(hostname, ipAddress)));
                    }));
                });

                IsRunning = false;
            });
        }

        private void RemoveHost(Guid hostId)
        {
            var index = -1;

            foreach (var host in Hosts)
            {
                if (host.HostId.Equals(hostId))
                    index = Hosts.IndexOf(host);
            }

            if (index != -1)
            {
                Hosts[index].CloseView();
                Hosts.RemoveAt(index);
            }
        }

        private void AddHostToHistory(string host)
        {
            // Create the new list
            var list = ListHelper.Modify(SettingsManager.Current.PingMonitor_HostHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PingMonitor_HostHistory.Clear();
            OnPropertyChanged(nameof(Host)); // Raise property changed again, after the collection has been cleared

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PingMonitor_HostHistory.Add(x));
        }

        private void StartDelayedSearch()
        {
            if (!IsSearching)
            {
                IsSearching = true;

                _searchDispatcherTimer.Start();
            }
            else
            {
                _searchDispatcherTimer.Stop();
                _searchDispatcherTimer.Start();
            }
        }

        private void StopDelayedSearch()
        {
            _searchDispatcherTimer.Stop();

            RefreshProfiles();

            IsSearching = false;
        }

        private void ResizeProfile(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.Profile_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix;
            }
            else
            {
                if (ExpandProfileView)
                {
                    ProfileWidth = Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.Profile_WidthCollapsed) < GlobalStaticConfiguration.FloatPointFix ? new GridLength(GlobalStaticConfiguration.Profile_DefaultWidthExpanded) : new GridLength(_tempProfileWidth);
                }
                else
                {
                    _tempProfileWidth = ProfileWidth.Value;
                    ProfileWidth = new GridLength(GlobalStaticConfiguration.Profile_WidthCollapsed);
                }
            }

            _canProfileWidthChange = true;
        }

        public void OnViewVisible()
        {
            _isViewActive = true;

            RefreshProfiles();
        }

        public void OnViewHide()
        {
            _isViewActive = false;
        }

        public void RefreshProfiles()
        {
            if (!_isViewActive)
                return;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                Profiles.Refresh();
            }));
        }

        public void OnProfileDialogOpen()
        {

        }

        public void OnProfileDialogClose()
        {

        }


        /// <summary>
        /// Method to display the status message and append messages related to <see cref="DNSClientResult"/>.
        /// </summary>
        /// <param name="host">Host which should be resolved.</param>
        /// <param name="result">Information about the error that occurred in the <see cref="DNSClientResult"/> query.</param>
        private void StatusMessageShowOrAdd(string host, DNSClientResult result)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                // Show the message
                if (!IsStatusMessageDisplayed)
                {
                    StatusMessage = DNSClientHelper.FormatDNSClientResultError(host, result);
                    IsStatusMessageDisplayed = true;

                    return;
                }

                // Append the message
                StatusMessage += Environment.NewLine + DNSClientHelper.FormatDNSClientResultError(host, result);
            }));
        }
        #endregion

        #region Event
        private void ProfileManager_OnProfilesUpdated(object sender, EventArgs e)
        {
            RefreshProfiles();
        }

        private void SearchDispatcherTimer_Tick(object sender, EventArgs e)
        {
            StopDelayedSearch();
        }
        #endregion
    }
}

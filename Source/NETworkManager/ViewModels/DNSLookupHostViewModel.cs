﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using NETworkManager.Controls;
using Dragablz;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Settings;
using NETworkManager.Views;
using NETworkManager.Utilities;
using NETworkManager.Profiles;
using System.Windows.Threading;
using NETworkManager.Models;
using System.Collections.Generic;

namespace NETworkManager.ViewModels
{
    public class DNSLookupHostViewModel : ViewModelBase, IProfileManager
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly DispatcherTimer _searchDispatcherTimer = new DispatcherTimer();

        public IInterTabClient InterTabClient { get; }
        public ObservableCollection<DragablzTabItem> TabItems { get; }

        private readonly bool _isLoading = true;
        private bool _isViewActive = true;

        private int _tabId;

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (value == _selectedTabIndex)
                    return;

                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        #region Profiles
        public ICollectionView Profiles { get; }

        private ProfileInfo _selectedProfile = new ProfileInfo();
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
                    SettingsManager.Current.DNSLookup_ExpandProfileView = value;

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
                    SettingsManager.Current.DNSLookup_ProfileWidth = value.Value;

                _profileWidth = value;

                if (_canProfileWidthChange)
                    ResizeProfile(true);

                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Constructor, load settings
        public DNSLookupHostViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;

            InterTabClient = new DragablzInterTabClient(ApplicationName.DNSLookup);

            TabItems = new ObservableCollection<DragablzTabItem>
            {
                new DragablzTabItem(Localization.Resources.Strings.NewTab, new DNSLookupView (_tabId), _tabId)
            };

            Profiles = new CollectionViewSource { Source = ProfileManager.Groups.SelectMany(x => x.Profiles) }.View;
            Profiles.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ProfileInfo.Group)));
            Profiles.SortDescriptions.Add(new SortDescription(nameof(ProfileInfo.Group), ListSortDirection.Ascending));
            Profiles.SortDescriptions.Add(new SortDescription(nameof(ProfileInfo.Name), ListSortDirection.Ascending));
            Profiles.Filter = o =>
            {
                if (!(o is ProfileInfo info))
                    return false;

                if (string.IsNullOrEmpty(Search))
                    return info.DNSLookup_Enabled;

                var search = Search.Trim();

                // Search by: Tag=xxx (exact match, ignore case)
                /*
                if (search.StartsWith(ProfileManager.TagIdentifier, StringComparison.OrdinalIgnoreCase))
                    return !string.IsNullOrEmpty(info.Tags) && info.DNSLookup_Enabled && info.Tags.Replace(" ", "").Split(';').Any(str => search.Substring(ProfileManager.TagIdentifier.Length, search.Length - ProfileManager.TagIdentifier.Length).Equals(str, StringComparison.OrdinalIgnoreCase));
                */

                // Search by: Name, DNSLookup_Host
                return info.DNSLookup_Enabled && (info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || info.DNSLookup_Host.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1);
            };

            // This will select the first entry as selected item...
            SelectedProfile = Profiles.SourceCollection.Cast<ProfileInfo>().Where(x => x.DNSLookup_Enabled).OrderBy(x => x.Group).ThenBy(x => x.Name).FirstOrDefault();

            ProfileManager.OnProfilesUpdated += ProfileManager_OnProfilesUpdated;

            _searchDispatcherTimer.Interval = GlobalStaticConfiguration.SearchDispatcherTimerTimeSpan;
            _searchDispatcherTimer.Tick += SearchDispatcherTimer_Tick;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            ExpandProfileView = SettingsManager.Current.DNSLookup_ExpandProfileView;

            ProfileWidth = ExpandProfileView ? new GridLength(SettingsManager.Current.DNSLookup_ProfileWidth) : new GridLength(GlobalStaticConfiguration.Profile_WidthCollapsed);

            _tempProfileWidth = SettingsManager.Current.DNSLookup_ProfileWidth;
        }
        #endregion

        #region ICommand & Actions
        public ICommand AddTabCommand => new RelayCommand(p => AddTabAction());

        private void AddTabAction()
        {
            AddTab();
        }

        public ICommand LookupProfileCommand => new RelayCommand(p => LookupProfileAction(), LookupProfile_CanExecute);

        private bool LookupProfile_CanExecute(object obj)
        {
            return !IsSearching && SelectedProfile != null;
        }

        private void LookupProfileAction()
        {
            AddTab(SelectedProfile.DNSLookup_Host);
        }

        public ICommand AddProfileCommand => new RelayCommand(p => AddProfileAction());

        private void AddProfileAction()
        {
            ProfileDialogManager.ShowAddProfileDialog(this, _dialogCoordinator, null, null, ApplicationName.DNSLookup);
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

        public ItemActionCallback CloseItemCommand => CloseItemAction;

        private static void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            ((args.DragablzItem.Content as DragablzTabItem)?.View as DNSLookupView)?.CloseTab();
        }
        #endregion

        #region Methods
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

        public void AddTab(string host = null)
        {
            _tabId++;

            TabItems.Add(new DragablzTabItem(host ?? Localization.Resources.Strings.NewTab, new DNSLookupView(_tabId, host), _tabId));

            SelectedTabIndex = TabItems.Count - 1;
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
﻿using System.Collections.ObjectModel;
using NETworkManager.Controls;
using Dragablz;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models.Settings;
using System.Collections.Generic;
using System.Linq;
using NETworkManager.Views;
using System.ComponentModel;
using System.Windows.Data;
using System;
using System.IO;
using NETworkManager.Utilities;
using System.Diagnostics;
using NETworkManager.Models.PuTTY;
using System.Windows;

namespace NETworkManager.ViewModels
{
    public class PuTTYHostViewModel : ViewModelBase
    {
        #region Variables
        private IDialogCoordinator dialogCoordinator;

        public IInterTabClient InterTabClient { get; private set; }
        public ObservableCollection<DragablzTabItem> TabItems { get; private set; }

        private const string tagIdentifier = "tag=";

        private bool _isLoading = true;

        private bool _isPuTTYConfigured;
        public bool IsPuTTYConfigured
        {
            get { return _isPuTTYConfigured; }
            set
            {
                if (value == _isPuTTYConfigured)
                    return;

                _isPuTTYConfigured = value;
                OnPropertyChanged();
            }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (value == _selectedTabIndex)
                    return;

                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }
                
        #region Profiles
        ICollectionView _profiles;
        public ICollectionView Profiles
        {
            get { return _profiles; }
        }

        private ProfileInfo _selectedProfile = new ProfileInfo();
        public ProfileInfo SelectedProfile
        {
            get { return _selectedProfile; }
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
            get { return _search; }
            set
            {
                if (value == _search)
                    return;

                _search = value;

                Profiles.Refresh();

                OnPropertyChanged();
            }
        }

        private bool _canProfileWidthChange = true;
        private double _tempProfileWidth;

        private bool _expandProfileView;
        public bool ExpandProfileView
        {
            get { return _expandProfileView; }
            set
            {
                if (value == _expandProfileView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.PuTTY_ExpandProfileView = value;

                _expandProfileView = value;

                if (_canProfileWidthChange)
                    ResizeProfile(dueToChangedSize: false);

                OnPropertyChanged();
            }
        }

        private GridLength _ProfileWidth;
        public GridLength ProfileWidth
        {
            get { return _ProfileWidth; }
            set
            {
                if (value == _ProfileWidth)
                    return;

                if (!_isLoading && value.Value != 40) // Do not save the size when collapsed
                    SettingsManager.Current.PuTTY_ProfileWidth = value.Value;

                _ProfileWidth = value;

                if (_canProfileWidthChange)
                    ResizeProfile(dueToChangedSize: true);

                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Constructor, load settings
        public PuTTYHostViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            // Check if putty is available...
            CheckIfPuTTYConfigured();

            InterTabClient = new DragablzInterTabClient(ApplicationViewManager.Name.PuTTY);

            TabItems = new ObservableCollection<DragablzTabItem>();

            _profiles = new CollectionViewSource { Source = ProfileManager.Profiles }.View;
            _profiles.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ProfileInfo.Group)));
            _profiles.SortDescriptions.Add(new SortDescription(nameof(ProfileInfo.Group), ListSortDirection.Ascending));
            _profiles.SortDescriptions.Add(new SortDescription(nameof(ProfileInfo.Name), ListSortDirection.Ascending));
            _profiles.Filter = o =>
            {
                ProfileInfo info = o as ProfileInfo;

                if (string.IsNullOrEmpty(Search))
                    return info.PuTTY_Enabled;

                string search = Search.Trim();

                // Search by: Name
                return (info.PuTTY_Enabled && info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1);
            };

            // This will select the first entry as selected item...
            SelectedProfile = Profiles.SourceCollection.Cast<ProfileInfo>().Where(x => x.PuTTY_Enabled).OrderBy(x => x.Group).ThenBy(x => x.Name).FirstOrDefault();
            
            LoadSettings();

            SettingsManager.Current.PropertyChanged += Current_PropertyChanged;
        }

        private void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsInfo.PuTTY_PuTTYLocation))
                CheckIfPuTTYConfigured();
        }

        private void LoadSettings()
        {
            ExpandProfileView = SettingsManager.Current.PuTTY_ExpandProfileView;

            if (ExpandProfileView)
                ProfileWidth = new GridLength(SettingsManager.Current.PuTTY_ProfileWidth);
            else
                ProfileWidth = new GridLength(40);

            _tempProfileWidth = SettingsManager.Current.PuTTY_ProfileWidth;
        }
        #endregion

        #region ICommand & Actions
        public ItemActionCallback CloseItemCommand
        {
            get { return CloseItemAction; }
        }

        private void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            ((args.DragablzItem.Content as DragablzTabItem).View as PuTTYControl).CloseTab();
        }

        public ICommand ConnectCommand
        {
            get { return new RelayCommand(p => ConnectAction()); }
        }

        private void ConnectAction()
        {
            Connect();
        }
                
        public ICommand ConnectProfileCommand
        {
            get { return new RelayCommand(p => ConnectProfileAction()); }
        }

        private void ConnectProfileAction()
        {
            ConnectProfile();
        }            

        public ICommand ConnectProfileExternalCommand
        {
            get { return new RelayCommand(p => ConnectProfileExternalAction()); }
        }

        private void ConnectProfileExternalAction()
        {
            ConnectProfileExternal();
        }

        public ICommand AddProfileCommand
        {
            get { return new RelayCommand(p => AddProfileAction()); }
        }

        private async void AddProfileAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_AddProfile")
            };

            ProfileViewModel profileViewModel = new ProfileViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;

                ProfileManager.AddProfile(instance);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;
            }, ProfileManager.GetGroups());

            customDialog.Content = new ProfileDialog
            {
                DataContext = profileViewModel
            };

            ConfigurationManager.Current.IsDialogOpen = true;
            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand EditProfileCommand
        {
            get { return new RelayCommand(p => EditProfileAction()); }
        }

        private async void EditProfileAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_EditProfile")
            };

            ProfileViewModel profileViewModel = new ProfileViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;

                ProfileManager.RemoveProfile(SelectedProfile);

                ProfileManager.AddProfile(instance);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;
            }, ProfileManager.GetGroups(), SelectedProfile);

            customDialog.Content = new ProfileDialog
            {
                DataContext = profileViewModel
            };

            ConfigurationManager.Current.IsDialogOpen = true;
            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand CopyAsProfileCommand
        {
            get { return new RelayCommand(p => CopyAsProfileAction()); }
        }

        private async void CopyAsProfileAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_CopyProfile")
            };

            ProfileViewModel profileViewModel = new ProfileViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;

                ProfileManager.AddProfile(instance);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;
            }, ProfileManager.GetGroups(), SelectedProfile);

            customDialog.Content = new ProfileDialog
            {
                DataContext = profileViewModel
            };

            ConfigurationManager.Current.IsDialogOpen = true;
            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand DeleteProfileCommand
        {
            get { return new RelayCommand(p => DeleteProfileAction()); }
        }

        private async void DeleteProfileAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_DeleteProfile")
            };

            ConfirmRemoveViewModel confirmRemoveViewModel = new ConfirmRemoveViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;

                ProfileManager.RemoveProfile(SelectedProfile);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;
            }, LocalizationManager.GetStringByKey("String_DeleteProfileMessage"));

            customDialog.Content = new ConfirmRemoveDialog
            {
                DataContext = confirmRemoveViewModel
            };

            ConfigurationManager.Current.IsDialogOpen = true;
            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand EditGroupCommand
        {
            get { return new RelayCommand(p => EditGroupAction(p)); }
        }

        private async void EditGroupAction(object group)
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_EditGroup")
            };

            GroupViewModel editGroupViewModel = new GroupViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;

                ProfileManager.RenameGroup(instance.OldGroup, instance.Group);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;
            }, group.ToString());

            customDialog.Content = new GroupDialog
            {
                DataContext = editGroupViewModel
            };

            ConfigurationManager.Current.IsDialogOpen = true;
            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand ClearSearchCommand
        {
            get { return new RelayCommand(p => ClearSearchAction()); }
        }

        private void ClearSearchAction()
        {
            Search = string.Empty;
        }

        public ICommand OpenSettingsCommand
        {
            get { return new RelayCommand(p => OpenSettingsAction()); }
        }

        private void OpenSettingsAction()
        {
            EventSystem.RedirectToSettings();
        }
        #endregion

        #region Methods
        private void CheckIfPuTTYConfigured()
        {
            if (!string.IsNullOrEmpty(SettingsManager.Current.PuTTY_PuTTYLocation))
                IsPuTTYConfigured = File.Exists(SettingsManager.Current.PuTTY_PuTTYLocation);
            else
                IsPuTTYConfigured = false;
        }

        private async void Connect(string host = null)
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = LocalizationManager.GetStringByKey("String_Header_Connect")
            };

            PuTTYConnectViewModel puTTYConnectViewModel = new PuTTYConnectViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;

                // Add host to history
                AddHostToHistory(instance.Host);
                AddSerialLineToHistory(instance.SerialLine);
                AddPortToHistory(instance.Port.ToString());
                AddBaudToHistory(instance.Baud.ToString());
                AddUsernameToHistory(instance.Username);
                AddProfileToHistory(instance.Profile);

                // Create Profile info
                Models.PuTTY.PuTTYProfileInfo puTTYProfileInfo = new Models.PuTTY.PuTTYProfileInfo
                {
                    HostOrSerialLine = instance.ConnectionMode == PuTTY.ConnectionMode.Serial ? instance.SerialLine : instance.Host,
                    Mode = instance.ConnectionMode,
                    PortOrBaud = instance.ConnectionMode == PuTTY.ConnectionMode.Serial ? instance.Baud : instance.Port,
                    Username = instance.Username,
                    Profile = instance.Profile,
                    AdditionalCommandLine = instance.AdditionalCommandLine
                };

                // Connect
                Connect(puTTYProfileInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.Current.IsDialogOpen = false;
            })
            {
                Host = host
            };

            customDialog.Content = new PuTTYConnectDialog
            {
                DataContext = puTTYConnectViewModel
            };

            ConfigurationManager.Current.IsDialogOpen = true;
            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        private void ConnectProfile()
        {
            Connect(Models.PuTTY.PuTTYProfileInfo.Parse(SelectedProfile), SelectedProfile.Name);
        }

        private void ConnectProfileExternal()
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = SettingsManager.Current.PuTTY_PuTTYLocation,
                Arguments = PuTTY.BuildCommandLine(Models.PuTTY.PuTTYProfileInfo.Parse(SelectedProfile))
            };

            Process.Start(info);
        }

        private void Connect(Models.PuTTY.PuTTYProfileInfo ProfileInfo, string Header = null)
        {
            // Add PuTTY path here...
            ProfileInfo.PuTTYLocation = SettingsManager.Current.PuTTY_PuTTYLocation;

            TabItems.Add(new DragablzTabItem(Header ?? ProfileInfo.HostOrSerialLine, new PuTTYControl(ProfileInfo)));

            SelectedTabIndex = TabItems.Count - 1;
        }

        public void AddTab(string host)
        {
            Connect(host);
        }

        // Modify history list
        private void AddHostToHistory(string host)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.PuTTY_HostHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PuTTY_HostHistory.Clear();

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PuTTY_HostHistory.Add(x));
        }

        private void AddSerialLineToHistory(string serialLine)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.PuTTY_SerialLineHistory.ToList(), serialLine, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PuTTY_SerialLineHistory.Clear();

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PuTTY_SerialLineHistory.Add(x));
        }

        private void AddPortToHistory(string host)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.PuTTY_PortHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PuTTY_PortHistory.Clear();

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PuTTY_PortHistory.Add(x));
        }

        private void AddBaudToHistory(string host)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.PuTTY_BaudHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PuTTY_BaudHistory.Clear();

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PuTTY_BaudHistory.Add(x));
        }

        private void AddUsernameToHistory(string host)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.PuTTY_UsernameHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PuTTY_UsernameHistory.Clear();

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PuTTY_UsernameHistory.Add(x));
        }

        private void AddProfileToHistory(string host)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.PuTTY_ProfileHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.PuTTY_ProfileHistory.Clear();

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.PuTTY_ProfileHistory.Add(x));
        }

        private void ResizeProfile(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                if (ProfileWidth.Value == 40)
                    ExpandProfileView = false;
                else
                    ExpandProfileView = true;
            }
            else
            {
                if (ExpandProfileView)
                {
                    if (_tempProfileWidth == 40)
                        ProfileWidth = new GridLength(250);
                    else
                        ProfileWidth = new GridLength(_tempProfileWidth);
                }
                else
                {
                    _tempProfileWidth = ProfileWidth.Value;
                    ProfileWidth = new GridLength(40);
                }
            }

            _canProfileWidthChange = true;
        }
        #endregion
    }
}
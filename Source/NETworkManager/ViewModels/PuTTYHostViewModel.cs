﻿using System.Collections.ObjectModel;
using NETworkManager.Controls;
using Dragablz;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Settings;
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
using NETworkManager.Profiles;
using System.Windows.Threading;
using NETworkManager.Models;
using NETworkManager.Models.EventSystem;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NETworkManager.ViewModels;

public class PuTTYHostViewModel : ViewModelBase, IProfileManager
{
    #region Variables
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly DispatcherTimer _searchDispatcherTimer = new();

    public IInterTabClient InterTabClient { get; }
    public ObservableCollection<DragablzTabItem> TabItems { get; }

    private readonly bool _isLoading = true;
    private bool _isViewActive = true;

    private bool _isConfigured;
    public bool IsConfigured
    {
        get => _isConfigured;
        set
        {
            if (value == _isConfigured)
                return;

            _isConfigured = value;
            OnPropertyChanged();
        }
    }

    private bool _disableFocusEmbeddedWindow;

    private DragablzTabItem _selectedTabItem;
    public DragablzTabItem SelectedTabItem
    {
        get => _selectedTabItem;
        set
        {
            if (value == _selectedTabItem)
                return;

            _selectedTabItem = value;

            // Focus embedded window on switching tab
            if (!_disableFocusEmbeddedWindow)
                FocusEmbeddedWindow();

            OnPropertyChanged();
        }
    }

    private bool _headerContextMenuIsOpen;
    public bool HeaderContextMenuIsOpen
    {
        get => _headerContextMenuIsOpen;
        set
        {
            if (value == _headerContextMenuIsOpen)
                return;

            _headerContextMenuIsOpen = value;
            OnPropertyChanged();
        }
    }

    #region Profiles
    public ICollectionView _profiles;
    public ICollectionView Profiles
    {
        get => _profiles;
        set
        {
            if (value == _profiles)
                return;

            _profiles = value;
            OnPropertyChanged();
        }
    }

    private ProfileInfo _selectedProfile = new();
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

            // Start searching...
            IsSearching = true;
            _searchDispatcherTimer.Start();

            OnPropertyChanged();
        }
    }

    private bool _textBoxSearchIsFocused;
    public bool TextBoxSearchIsFocused
    {
        get => _textBoxSearchIsFocused;
        set
        {
            if (value == _textBoxSearchIsFocused)
                return;

            _textBoxSearchIsFocused = value;
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
                SettingsManager.Current.PuTTY_ExpandProfileView = value;

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

            if (!_isLoading && Math.Abs(value.Value - GlobalStaticConfiguration.Profile_WidthCollapsed) > GlobalStaticConfiguration.Profile_FloatPointFix) // Do not save the size when collapsed
                SettingsManager.Current.PuTTY_ProfileWidth = value.Value;

            _profileWidth = value;

            if (_canProfileWidthChange)
                ResizeProfile(true);

            OnPropertyChanged();
        }
    }

    private bool _profileContextMenuIsOpen;
    public bool ProfileContextMenuIsOpen
    {
        get => _profileContextMenuIsOpen;
        set
        {

            if (value == _profileContextMenuIsOpen)
                return;

            _profileContextMenuIsOpen = value;
            OnPropertyChanged();
        }
    }
    #endregion
    #endregion

    #region Constructor, load settings
    public PuTTYHostViewModel(IDialogCoordinator instance)
    {
        _dialogCoordinator = instance;

        CheckSettings();

        InterTabClient = new DragablzInterTabClient(ApplicationName.PuTTY);

        TabItems = new ObservableCollection<DragablzTabItem>();
        TabItems.CollectionChanged += TabItems_CollectionChanged;

        // Profiles
        SetProfilesView();

        ProfileManager.OnProfilesUpdated += ProfileManager_OnProfilesUpdated;

        _searchDispatcherTimer.Interval = GlobalStaticConfiguration.SearchDispatcherTimerTimeSpan;
        _searchDispatcherTimer.Tick += SearchDispatcherTimer_Tick;

        LoadSettings();

        SettingsManager.Current.PropertyChanged += Current_PropertyChanged;

        _isLoading = false;
    }
    
    private void LoadSettings()
    {
        ExpandProfileView = SettingsManager.Current.PuTTY_ExpandProfileView;

        ProfileWidth = ExpandProfileView ? new GridLength(SettingsManager.Current.PuTTY_ProfileWidth) : new GridLength(GlobalStaticConfiguration.Profile_WidthCollapsed);

        _tempProfileWidth = SettingsManager.Current.PuTTY_ProfileWidth;
    }
    #endregion

   

    #region ICommand & Actions
    public ItemActionCallback CloseItemCommand => CloseItemAction;

    private void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
    {
        ((args.DragablzItem.Content as DragablzTabItem)?.View as PuTTYControl)?.CloseTab();
    }
    
    private bool Connect_CanExecute(object obj)
    {
        return IsConfigured;
    }

    public ICommand ConnectCommand => new RelayCommand(p => ConnectAction(), Connect_CanExecute);

    private void ConnectAction()
    {
        Connect();
    }

    private bool IsConnected_CanExecute(object view)
    {
        if (view is PuTTYControl control)
            return control.IsConnected;

        return false;
    }

    public ICommand ReconnectCommand => new RelayCommand(ReconnectAction);

    private void ReconnectAction(object view)
    {
        if (view is PuTTYControl control)
        {
            if (control.ReconnectCommand.CanExecute(null))
                control.ReconnectCommand.Execute(null);
        }
    }

    public ICommand ResizeWindowCommand => new RelayCommand(ResizeWindowAction, IsConnected_CanExecute);

    private void ResizeWindowAction(object view)
    {
        if (view is PuTTYControl control)
            control.ResizeEmbeddedWindow();
    }

    public ICommand RestartSessionCommand => new RelayCommand(RestartSessionAction, IsConnected_CanExecute);

    private void RestartSessionAction(object view)
    {
        if (view is PuTTYControl control)
            control.RestartSession();
    }
    
    public ICommand ConnectProfileCommand => new RelayCommand(p => ConnectProfileAction(), ConnectProfile_CanExecute);

    private bool ConnectProfile_CanExecute(object obj)
    {
        return !IsSearching && SelectedProfile != null;
    }

    private void ConnectProfileAction()
    {
        ConnectProfile();
    }

    public ICommand ConnectProfileExternalCommand => new RelayCommand(p => ConnectProfileExternalAction());

    private void ConnectProfileExternalAction()
    {
        ConnectProfileExternal();
    }

    public ICommand AddProfileCommand => new RelayCommand(p => AddProfileAction());

    private void AddProfileAction()
    {
        ProfileDialogManager.ShowAddProfileDialog(this, _dialogCoordinator, null, null, ApplicationName.PuTTY);
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

    public ICommand TextBoxSearchGotFocusCommand
    {
        get { return new RelayCommand(p => TextBoxSearchGotFocusAction()); }
    }

    private void TextBoxSearchGotFocusAction()
    {
        TextBoxSearchIsFocused = true;
    }

    public ICommand TextBoxSearchLostFocusCommand
    {
        get { return new RelayCommand(p => TextBoxSearchLostFocusAction()); }
    }

    private void TextBoxSearchLostFocusAction()
    {
        TextBoxSearchIsFocused = false;
    }

    public ICommand ClearSearchCommand => new RelayCommand(p => ClearSearchAction());

    private void ClearSearchAction()
    {
        Search = string.Empty;
    }

    public ICommand OpenSettingsCommand => new RelayCommand(p => OpenSettingsAction());

    private static void OpenSettingsAction()
    {
        EventSystem.RedirectToSettings();
    }
    #endregion

    #region Methods
    private void CheckSettings()
    {
        IsConfigured = !string.IsNullOrEmpty(SettingsManager.Current.PuTTY_ApplicationFilePath) && File.Exists(SettingsManager.Current.PuTTY_ApplicationFilePath);

        // Create default PuTTY profile for NETworkManager
        WriteDefaultProfileToRegistry();
    }

    private async Task Connect(string host = null)
    {
        var customDialog = new CustomDialog
        {
            Title = Localization.Resources.Strings.Connect
        };

        var connectViewModel = new PuTTYConnectViewModel(async instance =>
        {
            await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            ConfigurationManager.OnDialogClose();

            // Create profile info
            var info = new PuTTYSessionInfo
            {
                HostOrSerialLine = instance.ConnectionMode == ConnectionMode.Serial ? instance.SerialLine : instance.Host,
                Mode = instance.ConnectionMode,
                PortOrBaud = instance.ConnectionMode == ConnectionMode.Serial ? instance.Baud : instance.Port,
                Username = instance.Username,
                PrivateKey = instance.PrivateKeyFile,
                Profile = instance.Profile,
                EnableLog = SettingsManager.Current.PuTTY_EnableSessionLog,
                LogMode = SettingsManager.Current.PuTTY_LogMode,
                LogFileName = SettingsManager.Current.PuTTY_LogFileName,
                LogPath = Settings.Application.PuTTY.LogPath,
                AdditionalCommandLine = instance.AdditionalCommandLine
            };

            // Add to history
            // Note: The history can only be updated after the values have been read.
            //       Otherwise, in some cases, incorrect values are taken over.
            AddHostToHistory(instance.Host);
            AddSerialLineToHistory(instance.SerialLine);
            AddPortToHistory(instance.Port);
            AddBaudToHistory(instance.Baud);
            AddUsernameToHistory(instance.Username);
            AddPrivateKeyToHistory(instance.PrivateKeyFile);
            AddProfileToHistory(instance.Profile);

            Connect(info);
        }, async instance =>
            {
                await _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                ConfigurationManager.OnDialogClose();
            }, host);

        customDialog.Content = new PuTTYConnectDialog
        {
            DataContext = connectViewModel
        };

        ConfigurationManager.OnDialogOpen();
        await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
    }

    private void ConnectProfile()
    {
        Connect(NETworkManager.Profiles.Application.PuTTY.CreateSessionInfo(SelectedProfile), SelectedProfile.Name);
    }

    private void ConnectProfileExternal()
    {
        // Create log path
        DirectoryHelper.CreateWithEnvironmentVariables(Settings.Application.PuTTY.LogPath);

        var sessionInfo = NETworkManager.Profiles.Application.PuTTY.CreateSessionInfo(SelectedProfile);

        ProcessStartInfo info = new()
        {
            FileName = SettingsManager.Current.PuTTY_ApplicationFilePath,
            Arguments = PuTTY.BuildCommandLine(sessionInfo)
        };

        Process.Start(info);
    }

    private void Connect(PuTTYSessionInfo profileInfo, string header = null)
    {
        // Must be added here. So that it works with profiles and the connect dialog.
        profileInfo.ApplicationFilePath = SettingsManager.Current.PuTTY_ApplicationFilePath;

        TabItems.Add(new DragablzTabItem(header ?? profileInfo.HostOrSerialLine, new PuTTYControl(profileInfo)));

        // Select the added tab
        _disableFocusEmbeddedWindow = true;
        SelectedTabItem = TabItems.Last();
        _disableFocusEmbeddedWindow = false;
    }

    public void AddTab(string host)
    {
        Connect(host);
    }

    // Modify history list
    private static void AddHostToHistory(string host)
    {
        if (string.IsNullOrEmpty(host))
            return;

        SettingsManager.Current.PuTTY_HostHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_HostHistory.ToList(), host, SettingsManager.Current.General_HistoryListEntries));
    }

    private static void AddSerialLineToHistory(string serialLine)
    {
        if (string.IsNullOrEmpty(serialLine))
            return;

        SettingsManager.Current.PuTTY_SerialLineHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_SerialLineHistory.ToList(), serialLine, SettingsManager.Current.General_HistoryListEntries));
    }

    private static void AddPortToHistory(int port)
    {
        if (port == 0)
            return;

        SettingsManager.Current.PuTTY_PortHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_PortHistory.ToList(), port.ToString(), SettingsManager.Current.General_HistoryListEntries));
    }

    private static void AddBaudToHistory(int baud)
    {
        if (baud == 0)
            return;

        SettingsManager.Current.PuTTY_BaudHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_BaudHistory.ToList(), baud.ToString(), SettingsManager.Current.General_HistoryListEntries));
    }

    private static void AddUsernameToHistory(string username)
    {
        if (string.IsNullOrEmpty(username))
            return;

        SettingsManager.Current.PuTTY_UsernameHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_UsernameHistory.ToList(), username, SettingsManager.Current.General_HistoryListEntries));
    }

    private static void AddPrivateKeyToHistory(string privateKey)
    {
        if (string.IsNullOrEmpty(privateKey))
            return;

        SettingsManager.Current.PuTTY_PrivateKeyFileHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_PrivateKeyFileHistory.ToList(), privateKey, SettingsManager.Current.General_HistoryListEntries));
    }

    private static void AddProfileToHistory(string profile)
    {
        if (string.IsNullOrEmpty(profile))
            return;

        SettingsManager.Current.PuTTY_ProfileHistory = new ObservableCollection<string>(ListHelper.Modify(SettingsManager.Current.PuTTY_ProfileHistory.ToList(), profile, SettingsManager.Current.General_HistoryListEntries));
    }

    private void ResizeProfile(bool dueToChangedSize)
    {
        _canProfileWidthChange = false;

        if (dueToChangedSize)
        {
            ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.Profile_WidthCollapsed) > GlobalStaticConfiguration.Profile_FloatPointFix;
        }
        else
        {
            if (ExpandProfileView)
            {
                ProfileWidth = Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.Profile_WidthCollapsed) < GlobalStaticConfiguration.Profile_FloatPointFix ? new GridLength(GlobalStaticConfiguration.Profile_DefaultWidthExpanded) : new GridLength(_tempProfileWidth);
            }
            else
            {
                _tempProfileWidth = ProfileWidth.Value;
                ProfileWidth = new GridLength(GlobalStaticConfiguration.Profile_WidthCollapsed);
            }
        }

        _canProfileWidthChange = true;
    }

    public void FocusEmbeddedWindow()
    {
        /* Don't continue if
           - Search TextBox is focused
           - Header ContextMenu is opened
           - Profile ContextMenu is opened
        */
        if (TextBoxSearchIsFocused || HeaderContextMenuIsOpen || ProfileContextMenuIsOpen)
            return;

        (SelectedTabItem?.View as PuTTYControl)?.FocusEmbeddedWindow();
    }

    public void OnViewVisible()
    {
        _isViewActive = true;

        RefreshProfiles();

        FocusEmbeddedWindow();
    }

    public void OnViewHide()
    {
        _isViewActive = false;
    }

    private void SetProfilesView(ProfileInfo profile = null)
    {
        Profiles = new CollectionViewSource { Source = ProfileManager.Groups.SelectMany(x => x.Profiles).Where(x => x.PuTTY_Enabled).OrderBy(x => x.Group).ThenBy(x => x.Name) }.View;

        Profiles.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ProfileInfo.Group)));

        Profiles.Filter = o =>
        {
            if (o is not ProfileInfo info)
                return false;

            if (string.IsNullOrEmpty(Search))
                return true;

            var search = Search.Trim();

            // Search by: Tag=xxx (exact match, ignore case)
            /*
            if (search.StartsWith(ProfileManager.TagIdentifier, StringComparison.OrdinalIgnoreCase))
                return !string.IsNullOrEmpty(info.Tags) && info.PingMonitor_Enabled && info.Tags.Replace(" ", "").Split(';').Any(str => search.Substring(ProfileManager.TagIdentifier.Length, search.Length - ProfileManager.TagIdentifier.Length).Equals(str, StringComparison.OrdinalIgnoreCase));
            */

            // Search by: Name, PuTTY_HostOrSerialLine
            return info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || info.PuTTY_HostOrSerialLine.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1;
        };

        // Set specific profile or first if null
        SelectedProfile = null;

        if (profile != null)
            SelectedProfile = Profiles.Cast<ProfileInfo>().FirstOrDefault(x => x.Equals(profile)) ??
                Profiles.Cast<ProfileInfo>().FirstOrDefault();
        else
            SelectedProfile = Profiles.Cast<ProfileInfo>().FirstOrDefault();
    }

    public void RefreshProfiles()
    {
        if (!_isViewActive)
            return;

        SetProfilesView(SelectedProfile);
    }

    public void OnProfileManagerDialogOpen()
    {
        ConfigurationManager.OnDialogOpen();
    }

    public void OnProfileManagerDialogClose()
    {
        ConfigurationManager.OnDialogClose();
    }

    private void WriteDefaultProfileToRegistry()
    {
        if (IsConfigured)
            PuTTY.WriteDefaultProfileToRegistry(SettingsManager.Current.Appearance_Theme);
    }
    #endregion

    #region Event
    private void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingsInfo.PuTTY_ApplicationFilePath))
            CheckSettings();

        // Update PuTTY profile "NETworkManager" if application theme has changed
        if (e.PropertyName == nameof(SettingsInfo.Appearance_Theme))
            WriteDefaultProfileToRegistry();
    }

    private void ProfileManager_OnProfilesUpdated(object sender, EventArgs e)
    {
        RefreshProfiles();
    }

    private void SearchDispatcherTimer_Tick(object sender, EventArgs e)
    {
        _searchDispatcherTimer.Stop();

        RefreshProfiles();

        IsSearching = false;
    }

    private void TabItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        ConfigurationManager.Current.PuTTYHasTabs = TabItems.Count > 0;
    }
    #endregion
}
﻿using NETworkManager.Helpers;
using NETworkManager.Models.Settings;
using System.Net;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System;
using NETworkManager.Models.Network;
using System.ComponentModel;
using System.Windows.Data;
using NETworkManager.ViewModels.Dialogs;
using NETworkManager.Views.Dialogs;

namespace NETworkManager.ViewModels.Applications
{
    public class WakeOnLANViewModel : ViewModelBase
    {
        #region  Variables 
        private IDialogCoordinator dialogCoordinator;

        private bool _isLoading = true;

        private string _MACAddress;
        public string MACAddress
        {
            get { return _MACAddress; }
            set
            {
                if (value == _MACAddress)
                    return;

                _MACAddress = value;
                OnPropertyChanged();
            }
        }

        private bool _macAddressHasError;
        public bool MACAddressHasError
        {
            get { return _macAddressHasError; }
            set
            {
                if (value == _macAddressHasError)
                    return;

                _macAddressHasError = value;
                OnPropertyChanged();
            }
        }

        private string _broadcast;
        public string Broadcast
        {
            get { return _broadcast; }
            set
            {
                if (value == _broadcast)
                    return;

                _broadcast = value;
                OnPropertyChanged();
            }
        }

        private bool _broadcastHasError;
        public bool BroadcastHasError
        {
            get { return _broadcastHasError; }
            set
            {
                if (value == _broadcastHasError)
                    return;

                _broadcastHasError = value;
                OnPropertyChanged();
            }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                if (value == _port)
                    return;

                _port = value;
                OnPropertyChanged();
            }
        }

        private bool _portHasError;
        public bool PortHasError
        {
            get { return _portHasError; }
            set
            {
                if (value == _portHasError)
                    return;

                _portHasError = value;
                OnPropertyChanged();
            }
        }

        private bool _displayStatusMessage;
        public bool DisplayStatusMessage
        {
            get { return _displayStatusMessage; }
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
            get { return _statusMessage; }
            set
            {
                if (value == _statusMessage)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        #region Clients
        ICollectionView _wakeOnLANClients;
        public ICollectionView WakeOnLANClients
        {
            get { return _wakeOnLANClients; }
        }

        private WakeOnLANClientInfo _selectedClient;
        public WakeOnLANClientInfo SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                if (value == _selectedClient)
                    return;

                if (value != null)
                {
                    MACAddress = value.MACAddress;
                    Broadcast = value.Broadcast;
                    Port = value.Port;
                }

                _selectedClient = value;
                OnPropertyChanged();
            }
        }

        private bool _expandClientView;
        public bool ExpandClientView
        {
            get { return _expandClientView; }
            set
            {
                if (value == _expandClientView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.WakeOnLAN_ExpandClientView = value;

                _expandClientView = value;
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

                WakeOnLANClients.Refresh();

                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Constructor, load settings
        public WakeOnLANViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            if (WakeOnLANClientManager.Clients == null)
                WakeOnLANClientManager.Load();

            _wakeOnLANClients = CollectionViewSource.GetDefaultView(WakeOnLANClientManager.Clients);
            _wakeOnLANClients.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            _wakeOnLANClients.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            _wakeOnLANClients.Filter = o =>
            {
                if (string.IsNullOrEmpty(Search))
                    return true;

                WakeOnLANClientInfo info = o as WakeOnLANClientInfo;

                string search = Search.Trim();

                // Search by: Name
                return info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1;
            };

            LoadSettings();

            _isLoading = true;
        }

        private void LoadSettings()
        {
            Port = SettingsManager.Current.WakeOnLAN_DefaultPort;
            ExpandClientView = SettingsManager.Current.WakeOnLAN_ExpandClientView;
        }
        #endregion

        #region ICommands & Actions
        public ICommand WakeUpCommand
        {
            get { return new RelayCommand(p => WakeUpAction(), WakeUpAction_CanExecute); }
        }

        private bool WakeUpAction_CanExecute(object parameter)
        {
            return !MACAddressHasError && !BroadcastHasError && !PortHasError;
        }

        private void WakeUpAction()
        {
            WakeOnLANInfo info = new WakeOnLANInfo
            {
                MagicPacket = MagicPacketHelper.Create(MACAddress),
                Broadcast = IPAddress.Parse(Broadcast),
                Port = Port
            };

            WakeUp(info);
        }

        public ICommand WakeUpClientCommand
        {
            get { return new RelayCommand(p => WakeUpClientAction()); }
        }

        private void WakeUpClientAction()
        {
            WakeOnLANInfo info = new WakeOnLANInfo
            {
                MagicPacket = MagicPacketHelper.Create(SelectedClient.MACAddress),
                Broadcast = IPAddress.Parse(SelectedClient.Broadcast),
                Port = SelectedClient.Port
            };

            WakeUp(info);
        }

        public ICommand AddClientCommand
        {
            get { return new RelayCommand(p => AddClientAction()); }
        }

        private async void AddClientAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = Application.Current.Resources["String_Header_AddClient"] as string
            };

            WakeOnLANClientViewModel wakeOnLANClientViewModel = new WakeOnLANClientViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientInfo wakeOnLANClientInfo = new WakeOnLANClientInfo
                {
                    Name = instance.Name,
                    MACAddress = instance.MACAddress,
                    Broadcast = instance.Broadcast,
                    Port = instance.Port,
                    Group = instance.Group
                };

                WakeOnLANClientManager.AddClient(wakeOnLANClientInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, WakeOnLANClientManager.GetClientGroups(), new WakeOnLANClientInfo() { MACAddress = MACAddress, Broadcast = Broadcast, Port = Port });

            customDialog.Content = new WakeOnLANClientDialog
            {
                DataContext = wakeOnLANClientViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand EditClientCommand
        {
            get { return new RelayCommand(p => EditClientAction()); }
        }

        private async void EditClientAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = Application.Current.Resources["String_Header_EditClient"] as string
            };

            WakeOnLANClientViewModel wakeOnLANClientViewModel = new WakeOnLANClientViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientManager.RemoveClient(SelectedClient);

                WakeOnLANClientInfo wakeOnLANClientInfo = new WakeOnLANClientInfo
                {
                    Name = instance.Name,
                    MACAddress = instance.MACAddress,
                    Broadcast = instance.Broadcast,
                    Port = instance.Port,
                    Group = instance.Group
                };

                WakeOnLANClientManager.AddClient(wakeOnLANClientInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, WakeOnLANClientManager.GetClientGroups(), SelectedClient);

            customDialog.Content = new WakeOnLANClientDialog
            {
                DataContext = wakeOnLANClientViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand CopyAsClientCommand
        {
            get { return new RelayCommand(p => CopyAsProfileAction()); }
        }

        private async void CopyAsProfileAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = Application.Current.Resources["String_Header_CopyClient"] as string
            };

            WakeOnLANClientViewModel wakeOnLANClientViewModel = new WakeOnLANClientViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientInfo wakeOnLANClientInfo = new WakeOnLANClientInfo
                {
                    Name = instance.Name,
                    MACAddress = instance.MACAddress,
                    Broadcast = instance.Broadcast,
                    Port = instance.Port,
                    Group = instance.Group
                };

                WakeOnLANClientManager.AddClient(wakeOnLANClientInfo);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, WakeOnLANClientManager.GetClientGroups(), SelectedClient);

            customDialog.Content = new WakeOnLANClientDialog
            {
                DataContext = wakeOnLANClientViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public ICommand DeleteClientCommand
        {
            get { return new RelayCommand(p => DeleteClientAction()); }
        }

        private async void DeleteClientAction()
        {
            CustomDialog customDialog = new CustomDialog()
            {
                Title = Application.Current.Resources["String_Header_DeleteClient"] as string
            };

            ConfirmRemoveViewModel confirmRemoveViewModel = new ConfirmRemoveViewModel(instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);

                WakeOnLANClientManager.RemoveClient(SelectedClient);
            }, instance =>
            {
                dialogCoordinator.HideMetroDialogAsync(this, customDialog);
            }, Application.Current.Resources["String_DeleteClientMessage"] as string);

            customDialog.Content = new ConfirmRemoveDialog
            {
                DataContext = confirmRemoveViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }
        #endregion

        #region Methods
        private void WakeUp(WakeOnLANInfo info)
        {
            DisplayStatusMessage = false;
            StatusMessage = string.Empty;

            try
            {
                WakeOnLAN.Send(info);

                StatusMessage = Application.Current.Resources["String_MagicPacketSuccessfulSended"] as string;
                DisplayStatusMessage = true;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                DisplayStatusMessage = true;
            }
        }
        #endregion
    }
}

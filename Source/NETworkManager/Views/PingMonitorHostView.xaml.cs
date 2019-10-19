﻿using NETworkManager.ViewModels;
using System;
using NETworkManager.Models.Network;

namespace NETworkManager.Views
{
    public partial class PingMonitorHostView
    {
        private readonly PingMonitorHostViewModel _viewModel;

        public int HostId => _viewModel.HostId;

        public PingMonitorHostView(int hostId, Action<int> closeCallback, PingMonitorOptions options)
        {
            InitializeComponent();

            _viewModel = new PingMonitorHostViewModel(hostId, closeCallback, options);

            DataContext = _viewModel;

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.OnLoaded();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _viewModel.OnClose();
        }

        public void CloseView()
        {
            _viewModel.OnClose();
        }
    }
}

﻿using System;
using System.Windows.Controls;
using NETworkManager.ViewModels.Applications;

namespace NETworkManager.Views.Applications
{
    public partial class TracerouteView : UserControl
    {
        TracerouteViewModel viewModel = new TracerouteViewModel();

        public TracerouteView()
        {
            InitializeComponent();
            DataContext = viewModel;

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            viewModel.OnShutdown();
        }

        private void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = viewModel;
        }
    }
}
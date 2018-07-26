﻿using NETworkManager.ViewModels;

namespace NETworkManager.Views
{
    public partial class HTTPHeadersSettingsView
    {
        private readonly HTTPHeadersSettingsViewModel _viewModel = new HTTPHeadersSettingsViewModel();

        public HTTPHeadersSettingsView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}

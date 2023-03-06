﻿using NETworkManager.Models;
using NETworkManager.ViewModels;

namespace NETworkManager.Views;

public partial class SNMPHostView
{
    private readonly SNMPHostViewModel _viewModel = new();

    public SNMPHostView()
    {
        InitializeComponent();
        DataContext = _viewModel;

        InterTabController.Partition = ApplicationName.SNMP.ToString();
    }

    public void AddTab(string host)
    {
        _viewModel.AddTab(host);
    }

    public void OnViewHide()
    {
        _viewModel.OnViewHide();
    }

    public void OnViewVisible()
    {
        _viewModel.OnViewVisible();
    }
}

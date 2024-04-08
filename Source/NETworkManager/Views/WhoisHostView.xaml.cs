﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Models;
using NETworkManager.ViewModels;

namespace NETworkManager.Views;

public partial class WhoisHostView
{
    private readonly WhoisHostViewModel _viewModel = new(DialogCoordinator.Instance);

    public WhoisHostView()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu menu)
            menu.DataContext = _viewModel;
    }

    private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            _viewModel.QueryProfileCommand.Execute(null);
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
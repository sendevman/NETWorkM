﻿using Dragablz;
using MahApps.Metro.Controls;
using NETworkManager.Views;

namespace NETworkManager.Controls
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class DragablzPortScannerTabHostWindow : MetroWindow
    {        
        public DragablzPortScannerTabHostWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region ICommand & Actions
        public ItemActionCallback CloseItemCommand
        {
            get { return CloseItemAction; }
        }

        private void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            ((args.DragablzItem.Content as DragablzPortScannerTabItem).View as PortScannerView).CloseTab();
        }
        #endregion
    }
}
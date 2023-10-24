﻿// Contains code from: https://stackoverflow.com/questions/5028598/hosting-external-app-in-wpf-window

using System.Windows;
using System;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using NETworkManager.Utilities;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Settings;
using NETworkManager.Models.PowerShell;

namespace NETworkManager.Controls;

public partial class PowerShellControl : UserControlBase
{
    #region Variables
    private bool _initialized;
    private bool _closing;      // When the tab is closed --> OnClose()

    private readonly IDialogCoordinator _dialogCoordinator;

    private readonly PowerShellSessionInfo _sessionInfo;

    private Process _process;
    private IntPtr _appWin;

    private bool _isConnected;
    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (value == _isConnected)
                return;

            _isConnected = value;
            OnPropertyChanged();
        }
    }

    private bool _isConnecting;
    public bool IsConnecting
    {
        get => _isConnecting;
        set
        {
            if (value == _isConnecting)
                return;

            _isConnecting = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Constructor, load
    public PowerShellControl(PowerShellSessionInfo sessionInfo)
    {
        InitializeComponent();
        DataContext = this;

        _dialogCoordinator = DialogCoordinator.Instance;

        _sessionInfo = sessionInfo;

        Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Connect after the control is drawn and only on the first init
        if (_initialized)
            return;

        // Fix: The control is not visible by default, thus height and width is not set. If the values are not set, the size does not scale properly
        WindowHost.Height = (int)ActualHeight;
        WindowHost.Width = (int)ActualWidth;

        Connect().ConfigureAwait(false);
        _initialized = true;
    }

    private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
    {
        CloseTab();
    }
    #endregion

    #region ICommands & Actions
    public ICommand ReconnectCommand
    {
        get { return new RelayCommand(p => ReconnectAction()); }
    }

    private void ReconnectAction()
    {
        Reconnect();
    }
    #endregion

    #region Methods       
    private async Task Connect()
    {
        IsConnecting = true;

        var info = new ProcessStartInfo
        {
            FileName = _sessionInfo.ApplicationFilePath,
            Arguments = PowerShell.BuildCommandLine(_sessionInfo)
        };

        try
        {
            _process = Process.Start(info);

            if (_process != null)
            {
                _process.EnableRaisingEvents = true;
                _process.Exited += Process_Exited;

                // Embed window into panel, remove border etc.
                //  _process.WaitForInputIdle();
                _appWin = _process.MainWindowHandle;

                if (_appWin == IntPtr.Zero)
                {
                    var startTime = DateTime.Now;

                    while ((DateTime.Now - startTime).TotalSeconds < 10)
                    {
                        _process.Refresh();

                        if (_process.HasExited)
                            break;
                        

                        _appWin = _process.MainWindowHandle;

                        if (IntPtr.Zero != _appWin)
                            break;

                        await Task.Delay(100);
                    }
                }

                if (_appWin != IntPtr.Zero)
                {
                    NativeMethods.SetParent(_appWin, WindowHost.Handle);

                    // Show window before set style and resize
                    NativeMethods.ShowWindow(_appWin, NativeMethods.WindowShowStyle.Maximize);

                    // Remove border etc.
                    long style = (int)NativeMethods.GetWindowLong(_appWin, NativeMethods.GWL_STYLE);
                    style &= ~(NativeMethods.WS_CAPTION | NativeMethods.WS_POPUP | NativeMethods.WS_THICKFRAME);
                    NativeMethods.SetWindowLongPtr(_appWin, NativeMethods.GWL_STYLE, new IntPtr(style));
                    
                    IsConnected = true;

                    // Resize embedded application & refresh
                    // Requires a short delay because it's not applied immediately
                    await Task.Delay(250);
                    ResizeEmbeddedWindow();
                }
            }
            else
            {
                throw new Exception("Process could not be started!");
            }
        }
        catch (Exception ex)
        {
            if (!_closing)
            {
                var settings = AppearanceManager.MetroDialog;
                settings.AffirmativeButtonText = Localization.Resources.Strings.OK;

                ConfigurationManager.OnDialogOpen();

                await _dialogCoordinator.ShowMessageAsync(this, Localization.Resources.Strings.Error,
                    ex.Message, MessageDialogStyle.Affirmative, settings);

                ConfigurationManager.OnDialogClose();
            }
        }

        IsConnecting = false;
    }

    private void Process_Exited(object sender, EventArgs e)
    {
        // This happens when the user exit the process
        IsConnected = false;
    }

    public void FocusEmbeddedWindow()
    {
        if (IsConnected)
            NativeMethods.SetForegroundWindow(_process.MainWindowHandle);
    }

    public void ResizeEmbeddedWindow()
    {
        if (IsConnected)
            NativeMethods.SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, 0, 0, WindowHost.ClientSize.Width, WindowHost.ClientSize.Height, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
    }

    public void Disconnect()
    {
        if (IsConnected)
            _process.Kill();
    }

    private void Reconnect()
    {
        if (IsConnected)
            Disconnect();

        Connect().ConfigureAwait(false);
    }

    public void CloseTab()
    {
        _closing = true;

        Disconnect();
    }
    #endregion

    #region Events
    private void WindowGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (IsConnected)
            ResizeEmbeddedWindow();
    }
    #endregion
}
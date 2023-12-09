﻿using NETworkManager.Settings;

namespace NETworkManager.ViewModels;

public class PingMonitorSettingsViewModel : ViewModelBase
{
    #region Variables
    private readonly bool _isLoading;
    
    private int _timeout;
    public int Timeout
    {
        get => _timeout;
        set
        {
            if (value == _timeout)
                return;

            if (!_isLoading)
                SettingsManager.Current.PingMonitor_Timeout = value;

            _timeout = value;
            OnPropertyChanged();
        }
    }

    private int _buffer;
    public int Buffer
    {
        get => _buffer;
        set
        {
            if (value == _buffer)
                return;

            if (!_isLoading)
                SettingsManager.Current.PingMonitor_Buffer = value;

            _buffer = value;
            OnPropertyChanged();
        }
    }

    private int _ttl;
    public int TTL
    {
        get => _ttl;
        set
        {
            if (value == _ttl)
                return;

            if (!_isLoading)
                SettingsManager.Current.PingMonitor_TTL = value;

            _ttl = value;
            OnPropertyChanged();
        }
    }

    private bool _dontFragment;
    public bool DontFragment
    {
        get => _dontFragment;
        set
        {
            if (value == _dontFragment)
                return;

            if (!_isLoading)
                SettingsManager.Current.PingMonitor_DontFragment = value;

            _dontFragment = value;
            OnPropertyChanged();
        }
    }

    private int _waitTime;
    public int WaitTime
    {
        get => _waitTime;
        set
        {
            if (value == _waitTime)
                return;

            if (!_isLoading)
                SettingsManager.Current.PingMonitor_WaitTime = value;

            _waitTime = value;
            OnPropertyChanged();
        }
    }

    private bool _expandHostView;
    public bool ExpandHostView
    {
        get => _expandHostView;
        set
        {
            if (value == _expandHostView)
                return;

            if (!_isLoading)
                SettingsManager.Current.PingMonitor_ExpandHostView = value;

            _expandHostView = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Contructor, load settings
    public PingMonitorSettingsViewModel()
    {
        _isLoading = true;

        LoadSettings();

        _isLoading = false;
    }

    private void LoadSettings()
    {            
        Timeout = SettingsManager.Current.PingMonitor_Timeout;
        Buffer = SettingsManager.Current.PingMonitor_Buffer;
        TTL = SettingsManager.Current.PingMonitor_TTL;
        DontFragment = SettingsManager.Current.PingMonitor_DontFragment;
        WaitTime = SettingsManager.Current.PingMonitor_WaitTime;
        ExpandHostView = SettingsManager.Current.PingMonitor_ExpandHostView;
    }
    #endregion
}

﻿using NETworkManager.Settings;
using MahApps.Metro.Controls.Dialogs;
using System;
using NETworkManager.Models.Network;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using NETworkManager.Utilities;
using System.Windows.Data;
using MahApps.Metro.Controls;
using System.Windows.Input;
using System.Windows;
using log4net;

namespace NETworkManager.ViewModels;

public class BitCalculatorViewModel : ViewModelBase
{
    #region  Variables 
    private static readonly ILog Log = LogManager.GetLogger(typeof(BitCalculatorViewModel));
    private readonly IDialogCoordinator _dialogCoordinator;

    private readonly bool _isLoading = true;
    private bool _isViewActive = true;

    private string _input;
    public string Input
    {
        get => _input;
        set
        {
            if (value == _input)
                return;

            _input = value;
            OnPropertyChanged();
        }
    }

    public ICollectionView InputHistoryView { get; }

    private List<BitCaluclatorUnit> _units = new();
    public List<BitCaluclatorUnit> Units
    {
        get => _units;
        set
        {
            if (value == _units)
                return;

            _units = value;
            OnPropertyChanged();
        }
    }

    private BitCaluclatorUnit _unit;
    public BitCaluclatorUnit Unit
    {
        get => _unit;
        set
        {
            if (value == _unit)
                return;

            if (!_isLoading)
                SettingsManager.Current.BitCalculator_Unit = value;

            _unit = value;
            OnPropertyChanged();
        }
    }

    private bool _isCalculationRunning;
    public bool IsCalculationRunning
    {
        get => _isCalculationRunning;
        set
        {
            if (value == _isCalculationRunning)
                return;

            _isCalculationRunning = value;
            OnPropertyChanged();
        }
    }

    private bool _isResultVisible;
    public bool IsResultVisible
    {
        get => _isResultVisible;
        set
        {
            if (value == _isResultVisible)
                return;


            _isResultVisible = value;
            OnPropertyChanged();
        }
    }

    private BitCaluclatorInfo _result = new();
    public BitCaluclatorInfo Result
    {
        get => _result;
        set
        {
            if (value == _result)
                return;

            _result = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Constructor, load settings
    public BitCalculatorViewModel(IDialogCoordinator instance)
    {
        _dialogCoordinator = instance;

        InputHistoryView = CollectionViewSource.GetDefaultView(SettingsManager.Current.BitCalculator_InputHistory);

        Units = Enum.GetValues(typeof(BitCaluclatorUnit)).Cast<BitCaluclatorUnit>().ToList();
        Unit = Units.First(x => x == SettingsManager.Current.BitCalculator_Unit);

        LoadSettings();

        _isLoading = false;
    }

    private void LoadSettings()
    {


    }
    #endregion

    #region ICommands & Actions
    public ICommand CalculateCommand => new RelayCommand(p => CalcualateAction(), Calculate_CanExecute);

    private bool Calculate_CanExecute(object paramter) => Application.Current.MainWindow != null && !((MetroWindow)Application.Current.MainWindow).IsAnyDialogOpen;

    private void CalcualateAction()
    {
        Calculate();
    }
    #endregion

    #region Methods
    private async void Calculate()
    {
        IsResultVisible = false;
        IsCalculationRunning = true;

        if (double.TryParse(Input.Replace('.', ','), out double input))
        {
            Result = await BitCaluclator.CalculateAsync(input, Unit, SettingsManager.Current.BitCalculator_Notation);
        }
        else
        {
            Log.Error($"Could not parse input \"{Input}\" into double!");
        }

        IsResultVisible = true;

        AddInputToHistory(Input);

        IsCalculationRunning = false;
    }

    private void AddInputToHistory(string input)
    {
        // Create the new list
        var list = ListHelper.Modify(SettingsManager.Current.BitCalculator_InputHistory.ToList(), input, SettingsManager.Current.General_HistoryListEntries);

        // Clear the old items
        SettingsManager.Current.BitCalculator_InputHistory.Clear();
        OnPropertyChanged(nameof(Input)); // Raise property changed again, after the collection has been cleared

        // Fill with the new items
        list.ForEach(x => SettingsManager.Current.BitCalculator_InputHistory.Add(x));
    }

    public void OnViewVisible()
    {
        _isViewActive = true;
    }

    public void OnViewHide()
    {
        _isViewActive = false;
    }


    #endregion

    #region Event

    #endregion
}

﻿using System;
using System.Globalization;
using System.Windows.Data;
using NETworkManager.Profiles;

namespace NETworkManager.Converters;

public sealed class IsProfilesLocationToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value as string == ProfileManager.GetProfilesFolderLocation();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

﻿using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Data;
using NETworkManager.Localization;

namespace NETworkManager.Converters;

/// <summary>
/// Convert <see cref="IPStatus"/> to translated <see cref="string"/> or wise versa.
/// </summary>
public sealed class IPStatusToStringConverter : IValueConverter
{
    /// <summary>
    /// Convert <see cref="IPStatus"/> to translated <see cref="string"/>. 
    /// </summary>
    /// <param name="value">Object from type <see cref="IPStatus"/>.</param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns>Translated <see cref="IPStatus"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not IPStatus ipStatus
            ? "-/-"
            : ResourceTranslator.Translate(ResourceIdentifier.IPStatus, ipStatus);
    }

    /// <summary>
    /// !!! Method not implemented !!!
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

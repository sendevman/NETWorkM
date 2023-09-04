﻿using System;
using System.Globalization;
using System.Windows.Data;
using NETworkManager.Localization;
using NETworkManager.Models.Appearance;

namespace NETworkManager.Converters;

/// <summary>
/// Convert <see cref="AccentColorInfo"/> to translated <see cref="string"/> or wise versa.
/// </summary>
public sealed class AccentToStringConverter : IValueConverter
{
    /// <summary>
    /// Convert <see cref="AccentColorInfo"/> to translated <see cref="string"/>. 
    /// </summary>
    /// <param name="value">Object from type <see cref="AccentColorInfo"/>.</param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns>Translated <see cref="AccentColorInfo"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not string accent ? "-/-" : ResourceTranslator.Translate(ResourceIdentifier.Accent, accent);
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

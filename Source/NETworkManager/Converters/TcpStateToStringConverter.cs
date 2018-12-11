﻿using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Data;
using NETworkManager.Models.Settings;

namespace NETworkManager.Converters
{
    public sealed class TcpStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TcpState tcpState))
                return "-/-";

            var status = Resources.Localization.Strings.ResourceManager.GetString("TcpState_" + tcpState, LocalizationManager.Culture);

            return string.IsNullOrEmpty(status) ? tcpState.ToString() : status;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

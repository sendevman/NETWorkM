﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using NETworkManager.Settings;

namespace NETworkManager.Validators
{
    public class IsDNSLookupDNSServersNameUnique : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return SettingsManager.Current.DNSLookup_DNSServers.Any(x => string.Equals(x.Name, value as string, StringComparison.OrdinalIgnoreCase)) ? new ValidationResult(false, Localization.Resources.Strings.DNSServerWithThisNameAlreadyExists) : ValidationResult.ValidResult;
        }
    }
}

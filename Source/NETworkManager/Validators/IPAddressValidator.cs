﻿using NETworkManager.Models.Settings;
using NETworkManager.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace NETworkManager.Validators
{
    public class IPAddressValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null && (Regex.IsMatch((string)value, RegexHelper.IPv4AddressRegex) || Regex.IsMatch((string)value, RegexHelper.IPv6AddressRegex)))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, LocalizationManager.GetStringByKey("String_ValidationError_EnterValidIPAddress"));
        }
    }
}

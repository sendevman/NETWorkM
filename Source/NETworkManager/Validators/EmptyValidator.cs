﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NETworkManager.Validators
{
    public class EmptyValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value as string))
                return new ValidationResult(false, Application.Current.Resources["String_ValidateError_FieldEmpty"] as string);

            return ValidationResult.ValidResult;
        }
    }
}

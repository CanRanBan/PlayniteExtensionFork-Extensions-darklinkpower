﻿using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace PluginsCommon.Converters
{
    // Based on https://github.com/JosefNemec/Playnite
    #region ulong

    public class UlongToStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotSupportedException();
            }
            else if (value is ulong num)
            {
                return num.ToString();
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = (string)value;
            if (str.IsNullOrEmpty())
            {
                throw new NotSupportedException();
            }
            else
            {
                return ulong.Parse(str);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class UlongFieldValidation : ValidationRule
    {
        private string invalidInput => $"Not an ulong value in {MinValue} to {MaxValue} range!";

        public ulong MinValue { get; set; } = 0;
        public ulong MaxValue { get; set; } = ulong.MaxValue;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                var str = (string)value;
                if (str.IsNullOrEmpty())
                {
                    return new ValidationResult(false, invalidInput);
                }

                if (ulong.TryParse(str, out var intVal) && intVal >= MinValue && intVal <= MaxValue)
                {
                    return new ValidationResult(true, null);
                }

                return new ValidationResult(false, invalidInput);
            }
        }
    }

    #endregion ulong

    #region null int

    public class NullableIntToStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else if (value is int num)
            {
                return num.ToString();
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = (string)value;
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            else
            {
                return int.Parse(str);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class NullableIntFieldValidation : ValidationRule
    {
        private string invalidInput => $"Not an integer value in {MinValue} to {MaxValue} range!";

        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = int.MaxValue;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                var str = (string)value;
                if (str.IsNullOrEmpty())
                {
                    return new ValidationResult(true, null);
                }

                if (int.TryParse(str, out var intVal) && intVal >= MinValue && intVal <= MaxValue)
                {
                    return new ValidationResult(true, null);
                }

                return new ValidationResult(false, invalidInput);
            }
        }
    }

    #endregion null int

    #region double

    public class DoubleToStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotSupportedException();
            }
            else if (value is double num)
            {
                return num.ToString();
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = (string)value;
            if (str.IsNullOrEmpty())
            {
                throw new NotSupportedException();
            }
            else
            {
                return double.Parse(str);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class DoubleFieldValidation : ValidationRule
    {
        private string invalidInput => $"Not a double value in {MinValue} to {MaxValue} range!";

        public double MinValue { get; set; } = 0;
        public double MaxValue { get; set; } = double.MaxValue;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                var str = (string)value;
                if (str.IsNullOrEmpty())
                {
                    return new ValidationResult(false, invalidInput);
                }

                if (double.TryParse(str, out var doubleVal) && doubleVal >= MinValue && doubleVal <= MaxValue)
                {
                    return new ValidationResult(true, null);
                }

                return new ValidationResult(false, invalidInput);
            }
        }
    }

    #endregion double
}

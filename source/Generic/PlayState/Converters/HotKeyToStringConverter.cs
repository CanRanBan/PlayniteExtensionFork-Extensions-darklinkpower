﻿using Playnite.SDK;
using PlayState.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlayState.Converters
{
    public class HotKeyToStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is HotKey hotkey)
            {
                return hotkey.ToString();
            }

            return ResourceProvider.GetString("LOCPlayState_NoneLabel");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
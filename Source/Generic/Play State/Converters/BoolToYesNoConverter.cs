﻿using Playnite.SDK;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlayState.Converters
{
    public class BoolToYesNoConverter : MarkupExtension, IValueConverter
    {
        private readonly string yesString;
        private readonly string noString;

        public BoolToYesNoConverter()
        {
            yesString = ResourceProvider.GetString("LOCPlayStateYesLabel");
            noString = ResourceProvider.GetString("LOCPlayStateNoLabel");
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) == true ? yesString : noString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
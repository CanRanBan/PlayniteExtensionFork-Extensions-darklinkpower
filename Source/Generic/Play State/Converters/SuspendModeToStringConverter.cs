﻿using Playnite.SDK;
using PlayState.Enums;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlayState.Converters
{
    public class SuspendModeToStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var source = (SuspendModes)value;
            switch (source)
            {
                case SuspendModes.Processes:
                    return ResourceProvider.GetString("LOCPlayState_SuspendModeProcesses");
                case SuspendModes.Playtime:
                    return ResourceProvider.GetString("LOCPlayState_SuspendModePlaytime");
                case SuspendModes.Disabled:
                    return ResourceProvider.GetString("LOCPlayState_SuspendModeDisabled");
                default:
                    return string.Empty;
            }
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

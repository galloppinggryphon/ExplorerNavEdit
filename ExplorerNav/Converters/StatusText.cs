using ExplorerNav.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ExplorerNav.Converters
{
    [ValueConversion(typeof(NavItemState.StatusData), typeof(string))]
    internal class StatusText : IValueConverter
    {
        private readonly Dictionary<AllStatesEnum, string> statusTexts = new()
            {
                { AllStatesEnum.None, "" },
                { AllStatesEnum.Saved, "" },
                { AllStatesEnum.BuiltIn, "✓ Built-in" },
                { AllStatesEnum.Unsaved, "✕ Unapplied changes" },
                { AllStatesEnum.SavedBuiltIn, "" },
                { AllStatesEnum.UnsavedBuiltIn, "✕ Unapplied changes" },
                { AllStatesEnum.AppliedBuiltIn, "✓ Built-in" },
                { AllStatesEnum.UnappliedBuiltIn, "" },
                { AllStatesEnum.Applied, "✓ Registered" },
                { AllStatesEnum.Unapplied, "✕ Not registered" },
                { AllStatesEnum.UnappliedUnsaved, "✕ Not registered" },
                { AllStatesEnum.ReadError, "Read error!"},
                { AllStatesEnum.WriteError, "Write error!"}
            };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var states = (NavItemState.StatusData)value;
            var param = (string)parameter;
            var state = (AllStatesEnum)states.Data[param];

            return statusTexts[state];
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

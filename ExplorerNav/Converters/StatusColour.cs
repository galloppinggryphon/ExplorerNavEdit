using ExplorerNav.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ExplorerNav.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(NavItemState.StatusData))]
    internal class StatusColour : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var states = (NavItemState.StatusData)value;
            var data = states.Data;
            var applied = (AllStatesEnum)data["applied"];
            var saved = (AllStatesEnum)data["saved"];
            var error = (ErrorEnum)data["error"];

            string colourString;

            if (error != ErrorEnum.None)
                colourString = "#FF980000";
            //else if (applied == AppliedEnum.Unapplied || applied == AppliedEnum.None || saved == SavedEnum.Unsaved)
            else if (applied == AllStatesEnum.Unapplied || applied == AllStatesEnum.UnappliedBuiltIn || saved == AllStatesEnum.Unsaved)
                colourString = "#FFA79700";
            else
                colourString = "#FF0084B1";

            Color colour = (Color)ColorConverter.ConvertFromString(colourString);

            return new SolidColorBrush(colour);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}

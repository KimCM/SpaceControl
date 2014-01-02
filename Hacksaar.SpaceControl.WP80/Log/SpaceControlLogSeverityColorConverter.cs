using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Hacksaar.SpaceControl.WP80
{
    public class SpaceControlLogSeverityColorConverter : IValueConverter
    {
        // This converts the value object to the string to display.
        // This will work with most simple types.
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is SpaceControlLogSeverity))
                return value;

            var severity = (SpaceControlLogSeverity) value;
            
            switch (severity)
            {
                    case SpaceControlLogSeverity.Debug:
                    case SpaceControlLogSeverity.Undefined:
                    return new SolidColorBrush(Colors.DarkGray);
                    case SpaceControlLogSeverity.Info:
                    return new SolidColorBrush(Colors.LightGray);
                    case SpaceControlLogSeverity.Warning:
                    return new SolidColorBrush(Colors.Orange);
                    case SpaceControlLogSeverity.Error:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Gray);
            }
            
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

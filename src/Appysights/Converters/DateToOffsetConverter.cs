using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using ControlzEx.Theming;

namespace Appysights.Converters
{
    public class DateToOffsetConverter : IValueConverter
    {
        public static Theme CurrentTheme = ThemeManager.Current.DetectTheme();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;
            if (date == null)
            {
                return 1;
            }

            var maximumDate = DateTime.Now.AddHours(-3);
            var result = DateTime.Compare(maximumDate, date.Value);
            Debug.WriteLine($"[{result}] {date.Value} | {maximumDate}");
            return result > 0 ? -10 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

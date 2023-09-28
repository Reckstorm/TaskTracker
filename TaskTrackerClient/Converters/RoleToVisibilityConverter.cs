using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TaskTrackerClient.Converters
{
    class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            User principal = value as User;
            if (principal != null)
            {
                return principal.Role.RoleName.Equals((string)parameter) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            User principal = value as User;
            if (principal != null)
            {
                return principal.Role.RoleName.Equals((string)parameter) ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
    }
}

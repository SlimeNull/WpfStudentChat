using System.Globalization;
using System.Windows.Data;
using Microsoft.Extensions.DependencyInjection;
using WpfStudentChat.Services;

namespace WpfStudentChat.Converters
{
    internal class UserIdIsSelfConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int userId)
                return false;

            return userId == App.Host.Services.GetRequiredService<ChatClientService>().Client.GetUserId();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

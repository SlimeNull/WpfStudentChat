using System.Globalization;
using System.Windows.Data;

namespace WpfStudentChat.Converters
{
    internal class UserIdIsSelfConverter : IValueConverter
    {
        public int SelfUserId { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int userId)
                return false;

            return userId == SelfUserId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

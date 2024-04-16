using System.Globalization;
using System.Windows.Data;

namespace WpfStudentChat.Converters;

public class EqualsConverter : IValueConverter
{
    public static EqualsConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return object.Equals(value, parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WpfStudentChat.Converters;

public class MyStringToImageSourceConverter : IValueConverter
{
    public static MyStringToImageSourceConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value?.ToString();
        if (str is null)
            return null;

        try
        {
            return new BitmapImage(new Uri(str));
        }
        catch (Exception)
        {
            try
            {
                return new BitmapImage(new Uri($"file://{str}"));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

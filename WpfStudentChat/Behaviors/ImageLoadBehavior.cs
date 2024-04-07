using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;

namespace WpfStudentChat.Behaviors;

public class ImageLoadBehavior : Behavior<Image>
{
    public string ImageHash
    {
        get { return (string)GetValue(ImageHashProperty); }
        set { SetValue(ImageHashProperty, value); }
    }

    public static readonly DependencyProperty ImageHashProperty = DependencyProperty.Register("ImageHash", typeof(string), typeof(ImageLoadBehavior), new PropertyMetadata(string.Empty, async (dp, v) =>
    {
        ImageLoadBehavior obj = (ImageLoadBehavior)dp;

        var client = App.Host.Services.GetRequiredService<ChatClientService>();
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        MemoryStream bufferStream = new();
        using var stream = await client.Client.GetImageAsync(obj.ImageHash);
        if (stream is null)
        {
            obj.AssociatedObject.Source = null;
            return;
        }
        await stream.CopyToAsync(bufferStream);
        bitmap.StreamSource = bufferStream;
        bitmap.EndInit();
        obj.AssociatedObject.Source = bitmap;
    }));


    protected override void OnAttached() => ImageHash = ImageHash;
}

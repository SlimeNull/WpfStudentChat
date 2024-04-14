using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using WpfStudentChat.Services;

namespace WpfStudentChat.Behaviors;

public class ImageLoadBehavior : Behavior<BitmapImage>
{
    public string ImageHash
    {
        get { return (string)GetValue(ImageHashProperty); }
        set { SetValue(ImageHashProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ImageHash.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageHashProperty =
        DependencyProperty.Register("ImageHash", typeof(string), typeof(ImageLoadBehavior), new PropertyMetadata(string.Empty, ImageHashChangedCallback));

    private static async void ImageHashChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageLoadBehavior imageLoadBehavior)
            return;

        var client = App.Host.Services.GetRequiredService<ChatClientService>();

        try
        {
            MemoryStream bufferStream = new();
            using var stream = await client.Client.GetImageAsync(imageLoadBehavior.ImageHash);

            if (stream is null)
            {
                return;
            }

            await stream.CopyToAsync(bufferStream);

            if (bufferStream.Length == 0)
            {
                return;
            }

            bufferStream.Seek(0, SeekOrigin.Begin);
            imageLoadBehavior.AssociatedObject.BeginInit();
            imageLoadBehavior.AssociatedObject.StreamSource = bufferStream;
            imageLoadBehavior.AssociatedObject.EndInit();
        }
        catch { }
    }

    protected override void OnAttached()
    {
        ImageHashChangedCallback(this, default);
    }
}

﻿using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using WpfStudentChat.Services;

namespace WpfStudentChat.Behaviors;

public class BackgroundImageLoadBehavior : Behavior<Border>
{
    public int UserId
    {
        get { return (int)GetValue(UserIdProperty); }
        set { SetValue(UserIdProperty, value); }
    }

    public string ImageHash
    {
        get { return (string)GetValue(ImageHashProperty); }
        set { SetValue(ImageHashProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ImageHash.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageHashProperty =
        DependencyProperty.Register("ImageHash", typeof(string), typeof(BackgroundImageLoadBehavior), new PropertyMetadata(string.Empty, StatusChangedCallback));

    // Using a DependencyProperty as the backing store for UserId.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UserIdProperty =
        DependencyProperty.Register("UserId", typeof(int), typeof(BackgroundImageLoadBehavior), new PropertyMetadata(-1, StatusChangedCallback));

    private static async void StatusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BackgroundImageLoadBehavior imageLoadBehavior)
            return;

        var client = App.Host.Services.GetRequiredService<ChatClientService>();

        var imageHash = imageLoadBehavior.ImageHash;

        try
        {
            if (string.IsNullOrWhiteSpace(imageHash))
            {
                var user = await client.Client.GetUser(imageLoadBehavior.UserId);
                imageHash = user.AvatarHash;
            }

            MemoryStream bufferStream = new();
            using var stream = await client.Client.GetImageAsync(imageHash);

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
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = bufferStream;
            image.EndInit();

            imageLoadBehavior.AssociatedObject.Background = new ImageBrush()
            {
                ImageSource = image
            };
        }
        catch { }
    }

    protected override void OnAttached()
    {
        StatusChangedCallback(this, default);
    }
}
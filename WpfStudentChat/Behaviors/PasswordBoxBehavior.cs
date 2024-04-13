using Microsoft.Xaml.Behaviors;
using Wpf.Ui.Controls;

namespace WpfStudentChat.Behaviors;

public class PasswordBoxBehavior : Behavior<PasswordBox>
{
    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxBehavior), new PropertyMetadata(string.Empty, (dp, v) =>
    {
        if (dp is not PasswordBoxBehavior obj ||
            obj.AssociatedObject is null)
        {
            return;
        }

        var passwordBox = obj.AssociatedObject;
        passwordBox.Password = (string)v.NewValue;
    }));

    protected override void OnAttached()
    {
        AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
    }


    protected override void OnDetaching()
    {
        AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
    }


    private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
    {
        Password = AssociatedObject.Password;
    }

}

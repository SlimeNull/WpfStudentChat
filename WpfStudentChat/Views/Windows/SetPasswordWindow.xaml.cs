using WpfStudentChat.Services;

namespace WpfStudentChat.Views.Windows;

/// <summary>
/// Interaction logic for SetPasswordWindow.xaml
/// </summary>
[ObservableObject]
public partial class SetPasswordWindow : Window
{
    private readonly ChatClientService _chatClientService;

    [ObservableProperty] private string _oldPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _new2Password = string.Empty;

    public SetPasswordWindow(ChatClientService chatClientService)
    {
        DataContext = this;
        InitializeComponent();
        _chatClientService = chatClientService;
    }


    [RelayCommand]
    private async Task Ok()
    {
        if (Math.Min(NewPassword.Length, New2Password.Length) < 6)
        {
            MessageBox.Show("密码不能小于6位", "警告", icon: MessageBoxImage.Warning, button: MessageBoxButton.OK);
            return;
        }

        if(NewPassword != New2Password)
        {
            MessageBox.Show("两次输入的密码不一致", "警告", icon: MessageBoxImage.Warning, button: MessageBoxButton.OK);
            return;
        }

        try
        {
            await _chatClientService.Client.SetSelfPasswordAsync(OldPassword, NewPassword);
            MessageBox.Show("修改成功");
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"修改失败 {ex.Message}");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Close();
    }
}

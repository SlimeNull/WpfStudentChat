using CommunityToolkit.Mvvm.Messaging;
using WpfStudentChat.Models.Messages;

namespace WpfStudentChat.ViewModels.Windows;

public partial class LoginWindowViewModel(IMessenger messenger) : ObservableObject
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    public bool IsLogged { get; set; }

    [RelayCommand]
    public async Task Login()
    {
        MessageBox.Show($"用户名: {Username}\n密码: {Password}");

        IsLogged = true;
        messenger.Send(new LoggedMessage());

        await Task.CompletedTask;
    }
}

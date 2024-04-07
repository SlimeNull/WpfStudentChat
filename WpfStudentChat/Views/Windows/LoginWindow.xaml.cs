using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : FluentWindow
{
    private readonly ChatClientService _chatClientService;

    public LoginWindowViewModel ViewModel { get; }

    public LoginWindow(
        LoginWindowViewModel viewModel,
        ChatClientService chatClientService)
    {

        ViewModel = viewModel;
        _chatClientService = chatClientService;
        DataContext = this;

        InitializeComponent();
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        try
        {
            await _chatClientService.Client.LoginAsync(ViewModel.Username, ViewModel.Password);

            DialogResult = true;
            Close();
        }
        catch (Exception)
        {
            System.Windows.MessageBox.Show("登录失败");
            return;
        }
    }
}

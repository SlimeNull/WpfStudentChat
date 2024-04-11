using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : UiWindow
{
    private readonly ChatClientService _chatClientService;

    public LoginViewModel ViewModel { get; }

    public LoginWindow(
        LoginViewModel viewModel,
        ChatClientService chatClientService)
    {

        ViewModel = viewModel;
        _chatClientService = chatClientService;
        DataContext = this;

        InitializeComponent();

        Wpf.Ui.Appearance.Watcher.Watch(this);
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
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(this, $"Failed to login. {ex.Message}", "Error");
            return;
        }
    }
}

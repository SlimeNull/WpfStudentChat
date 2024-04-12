using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ManagePage.xaml
/// </summary>
public partial class ManagePage : Page
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ChatClientService _chatClientService;

    public ManageViewModel ViewModel { get; }

    public ManagePage(
        ManageViewModel viewModel, 
        IServiceProvider serviceProvider,
        ChatClientService chatClientService)
    {
        ViewModel = viewModel;
        _serviceProvider = serviceProvider;
        _chatClientService = chatClientService;
        DataContext = this;
        InitializeComponent();
        Loaded += ManagePage_Loaded;
    }

    private async void ManagePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadUsers();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (((DataGridRow)sender).Item is not User user)
            return;

        using var scope = _serviceProvider.CreateScope();
        var win = scope.ServiceProvider.GetRequiredService<EditUserWindow>();
        win.User = user with { };
        win.Owner = App.Current.MainWindow;
        win.OnPropertyChanged(nameof(win.User));

        if (win.ShowDialog() is true)
        {
            if(ViewModel.Users.IndexOf(user) is int index && index >= 0)
            {
                ViewModel.Users[index] = win.User;
            }
        }
    }

    [RelayCommand]
    public async Task DeleteSelectedUsers()
    {
        var users = userDataGrid.SelectedItems.Cast<User>().ToArray();

        foreach (var user in users)
        {
            try
            {
                await _chatClientService.Client.DeleteUser(user.Id);
                ViewModel.Users.Remove(user);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除失败 Id:{user.Id}");
                break;
            }
        }
    }
}

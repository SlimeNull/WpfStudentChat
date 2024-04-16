using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using Wpf.Ui.Mvvm.Contracts;
using WpfStudentChat.Services;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsFriendPage.xaml
/// </summary>
public partial class ContactsFriendPage : Page
{
    public User User { get; }
    public ContactsFriendPage(User user)
    {
        User = user;
        DataContext = this;
        InitializeComponent();
    }

    [RelayCommand]
    public async Task StartChat()
    {
        var page = App.Host.Services.GetRequiredService<ChatPage>();
        var navigationService = App.Host.Services.GetRequiredService<INavigationService>();
        await page.EnsureSessionAsync(User);
        page.SelectSession(User);

        navigationService.Navigate(typeof(ChatPage));
    }

    [RelayCommand]
    public async Task DeleteContact()
    {
        var chatClientService = App.Host.Services.GetRequiredService<ChatClientService>();

        if (MessageBox.Show(App.Current.MainWindow, "您确定要删除该好友吗", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        await chatClientService.Client.DeleteFriendAsync(User.Id);
    }
}

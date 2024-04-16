using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using Wpf.Ui.Mvvm.Contracts;
using WpfStudentChat.Services;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsGroupPage.xaml
/// </summary>
public partial class ContactsGroupPage : Page
{
    public Group Group { get; }
    public ContactsGroupPage(Group group)
    {
        Group = group;
        DataContext = this;
        InitializeComponent();
    }

    [RelayCommand]
    public async Task StartChat()
    {
        var page = App.Host.Services.GetRequiredService<ChatPage>();
        var navigationService = App.Host.Services.GetRequiredService<INavigationService>();
        await page.EnsureSessionAsync(Group);
        page.SelectSession(Group);

        navigationService.Navigate(typeof(ChatPage));
    }

    [RelayCommand]
    public async Task DeleteContact()
    {
        var chatClientService = App.Host.Services.GetRequiredService<ChatClientService>();

        if (MessageBox.Show(App.Current.MainWindow, "您确定要删除该群吗", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        await chatClientService.Client.DeleteGroupAsync(Group.Id);
    }
}

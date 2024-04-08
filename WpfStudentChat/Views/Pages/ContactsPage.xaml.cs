using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsPage.xaml
/// </summary>
public partial class ContactsPage : Page, INavigableView<ContactsViewModel>
{
    private readonly ChatClientService _chatClientService;
    private readonly IServiceProvider _serviceProvider;
    private readonly INavigationService _navigationService;

    public ContactsViewModel ViewModel { get; }
    public ContactsPage(
        ContactsViewModel viewModel,
        ChatClientService chatClientService,
        IServiceProvider serviceProvider,
        INavigationService navigationService)
    {
        _chatClientService = chatClientService;
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    [RelayCommand]
    public async Task LoadFriendsAndGroups()
    {
        try
        {
            var friends = await _chatClientService.Client.GetFriends();
            var groups = await _chatClientService.Client.GetGroups();

            ViewModel.Friends.Clear();
            foreach (var friend in friends)
                ViewModel.Friends.Add(friend);

            ViewModel.Groups.Clear();
            foreach (var group in groups)
                ViewModel.Groups.Add(group);
        }
        catch
        {
            System.Windows.MessageBox.Show(Application.Current.MainWindow, "Faild to load friends and groups", "Error");
        }
    }

    [RelayCommand]
    public void StartPrivateChat(User user)
    {
        var chatPage = _serviceProvider.GetRequiredService<ChatPage>();
        chatPage.EnsureSession(user);
        chatPage.SelectSession(user);

        _navigationService.Navigate(typeof(ChatPage));
    }

    [RelayCommand]
    public void StartGroupChat(Group group)
    {
        var chatPage = _serviceProvider.GetRequiredService<ChatPage>();
        chatPage.EnsureSession(group);
        chatPage.SelectSession(group);

        _navigationService.Navigate(typeof(ChatPage));
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}

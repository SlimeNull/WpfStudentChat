using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using Wpf.Ui;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using WpfStudentChat.Extensions;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsPage.xaml
/// </summary>
public partial class ContactsPage : Page, INavigableView<ContactsViewModel>,
    IRecipient<FriendIncreasedMessage>,
    IRecipient<FriendDecreasedMessage>,
    IRecipient<GroupIncreasedMessage>,
    IRecipient<GroupDecreasedMessage>
{
    private readonly ChatClientService _chatClientService;
    private readonly IServiceProvider _serviceProvider;
    private readonly INavigationService _navigationService;

    public ContactsViewModel ViewModel { get; }
    public ContactsPage(
        ContactsViewModel viewModel,
        ChatClientService chatClientService,
        IServiceProvider serviceProvider,
        INavigationService navigationService,
        IMessenger messenger)
    {
        _chatClientService = chatClientService;
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        messenger.Register<FriendIncreasedMessage>(this);
        messenger.Register<FriendDecreasedMessage>(this);
        messenger.Register<GroupIncreasedMessage>(this);
        messenger.Register<GroupDecreasedMessage>(this);
    }

    [RelayCommand]
    public async Task LoadFriendsAndGroups()
    {
        try
        {
            var friends = await _chatClientService.Client.GetFriendsAsync();
            var groups = await _chatClientService.Client.GetGroupsAsync();

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
    public async Task StartPrivateChat(User user)
    {
        var chatPage = _serviceProvider.GetRequiredService<ChatPage>();
        await chatPage.EnsureSessionAsync(user);
        chatPage.SelectSession(user);

        _navigationService.Navigate(typeof(ChatPage));
    }

    [RelayCommand]
    public async Task StartGroupChat(Group group)
    {
        var chatPage = _serviceProvider.GetRequiredService<ChatPage>();
        await chatPage.EnsureSessionAsync(group);
        chatPage.SelectSession(group);

        _navigationService.Navigate(typeof(ChatPage));
    }

    [RelayCommand]
    public void ShowSearchWindow()
    {
        using var scope = _serviceProvider.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<SearchWindow>();
        window.Owner = App.Current.MainWindow;
        window.ShowDialog();
    }

    [RelayCommand]
    public void ShowCreateGroupWindow()
    {
        using var scope = _serviceProvider.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<SetGroupProfileWindow>();
        window.Owner = App.Current.MainWindow;
        window.ShowDialog();
    }

    [RelayCommand]
    public void ShowFriendRequestsPage()
    {
        var page = _serviceProvider.GetRequiredService<FriendRequestsPage>();
        ContentFrame.Content = page;
    }

    [RelayCommand]
    public void ShowGroupRequestsPage()
    {
        var page = _serviceProvider.GetRequiredService<GroupRequestsPage>();
        ContentFrame.Content = page;
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListView senderListView)
        {
            return;
        }

        if (FriendsListView.SelectedItem is null &&
            GroupsListView.SelectedItem is null)
        {
            ContentFrame.Content = null;
        }

        if (senderListView.SelectedItem is null)
        {
            return;
        }

        if (sender == FriendsListView)
        {
            GroupsListView.SelectedItem = null;

            var page = new ContactsFriendPage((User)senderListView.SelectedItem);
            ContentFrame.Content = page;
        }
        else if (sender == GroupsListView)
        {
            FriendsListView.SelectedItem = null;

            var page = new ContactsGroupPage((Group)senderListView.SelectedItem);
            ContentFrame.Content = page;
        }
    }

    void IRecipient<FriendIncreasedMessage>.Receive(FriendIncreasedMessage message)
    {
        ViewModel.Friends.Add(message.Friend);
    }

    void IRecipient<FriendDecreasedMessage>.Receive(FriendDecreasedMessage message)
    {
        int index = ViewModel.Friends.FindIndex(friend => friend.Id == message.Friend.Id);

        if (index != -1)
            ViewModel.Friends.RemoveAt(index);
    }

    void IRecipient<GroupIncreasedMessage>.Receive(GroupIncreasedMessage message)
    {
        ViewModel.Groups.Add(message.Group);
    }

    void IRecipient<GroupDecreasedMessage>.Receive(GroupDecreasedMessage message)
    {
        int index = ViewModel.Groups.FindIndex(group => group.Id == message.Group.Id);

        if (index != -1)
            ViewModel.Groups.RemoveAt(index);
    }

    private void ContactsListView_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (!e.Handled)
        {
            // ListView拦截鼠标滚轮事件
            e.Handled = true;

            // 激发一个鼠标滚轮事件，冒泡给外层ListView接收到
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            var parent = ((Control)sender).Parent as UIElement;

            if (parent is not null)
                parent.RaiseEvent(eventArg);
        }
    }
}

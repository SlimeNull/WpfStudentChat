using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using StudentChat.Models;
using WpfStudentChat.Models;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages
{
    /// <summary>
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page,
        IRecipient<PrivateMessageReceivedMessage>,
        IRecipient<GroupMessageReceivedMessage>
    {
        private readonly ChatClientService _chatClientService;

        public ChatPage(
            ChatViewModel viewModel,
            ChatClientService chatClientService,
            IMessenger messenger)
        {
            SelfUserId = chatClientService.Client.GetSelfUserId();
            ViewModel = viewModel;
            _chatClientService = chatClientService;
            DataContext = this;

            InitializeComponent();

            messenger.Register<PrivateMessageReceivedMessage>(this);
            messenger.Register<GroupMessageReceivedMessage>(this);

            Loaded += ChatPage_Loaded;
        }

        private bool _isLoaded = false;
        private async void ChatPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
                return;
            _isLoaded = true;

            var groups = await _chatClientService.Client.GetGroupsAsync();
            var friends = await _chatClientService.Client.GetFriendsAsync();

            var groupLoad = Parallel.ForEachAsync(groups, async (group, cancellationToken) =>
            {
                if (ViewModel.GetGroupSession(group.Id) is null)
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        var newSession = await ViewModel.AddGroupSessionAsync(group.Id);
                        newSession.LastReadTime = await _chatClientService.Client.GetGroupMessageLastTime(group.Id);
                        var messages = await _chatClientService.Client.QueryGroupMessagesAsync(group.Id, DateTimeOffset.Now.AddDays(-30), DateTimeOffset.Now, 100);
                        var unreadCount = messages.Count(v => v.SentTime >= newSession.LastReadTime);
                        newSession.UnreadMessageCount = unreadCount;
                        foreach (var message in messages)
                        {
                            newSession.Messages.Add(message);
                        }
                    });
                }
            });

            var friendLoad = Parallel.ForEachAsync(friends, async (friend, cancellationToken) =>
            {
                if (ViewModel.GetPrivateSession(friend.Id) is null)
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        var newSession = await ViewModel.AddPrivateSessionAsync(friend.Id);
                        newSession.LastReadTime = await _chatClientService.Client.GetFriendMessageLastTime(friend.Id);
                        var messages = await _chatClientService.Client.QueryPrivateMessagesAsync(friend.Id, DateTimeOffset.Now.AddDays(-30), DateTimeOffset.Now, 100);
                        var unreadCount = messages.Count(v => v.SentTime >= newSession.LastReadTime);
                        newSession.UnreadMessageCount = unreadCount;
                        foreach (var message in messages)
                        {
                            newSession.Messages.Add(message);
                        }
                    });
                }
            });

            await groupLoad;
            await friendLoad;
        }

        public int SelfUserId { get; }
        public ChatViewModel ViewModel { get; }

        public async Task<IChatSession> EnsureSessionAsync(IIdentifiable identifiable)
        {
            if (ViewModel.GetSession(identifiable) is IChatSession existChatSession)
                return existChatSession;

            return await ViewModel.AddSessionAsync(identifiable);
        }

        public void SelectSession(IIdentifiable identifiable)
        {
            ViewModel.SelectedSession = ViewModel.GetSession(identifiable);
        }

        [RelayCommand]
        public void DeleteSession(IChatSession session)
        {
            ViewModel.Sessions.Remove(session);
            if (ViewModel.SelectedSession == session)
                ViewModel.SelectedSession = null;
        }

        async void IRecipient<PrivateMessageReceivedMessage>.Receive(PrivateMessageReceivedMessage message)
        {
            if (message.Message.SenderId == SelfUserId)
            {
                return;
            }

            if (ViewModel.GetPrivateSession(message.Message.SenderId) is null)
            {
                var newSession = await ViewModel.AddPrivateSessionAsync(message.Message.SenderId);
                newSession.Messages.Add(message.Message);
            }
        }

        async void IRecipient<GroupMessageReceivedMessage>.Receive(GroupMessageReceivedMessage message)
        {
            if (ViewModel.GetGroupSession(message.Message.GroupId) is null)
            {
                var newSession = await ViewModel.AddGroupSessionAsync(message.Message.GroupId);
                newSession.Messages.Add(message.Message);
            }
        }
    }
}

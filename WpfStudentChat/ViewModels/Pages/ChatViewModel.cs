using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using WpfStudentChat.Models;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;
using WpfStudentChat.Views.Pages;

namespace WpfStudentChat.ViewModels.Pages;

public partial class ChatViewModel : ObservableObject, IRecipient<PrivateMessageReceivedMessage>, IRecipient<GroupMessageReceivedMessage>
{
    private readonly ChatClientService _chatClientService;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<IChatSession, Page> _cachedMessagesPages = new();

    public ChatViewModel(
        ChatClientService chatClientService,
        IServiceProvider serviceProvider,
        IMessenger messenger)
    {
        _chatClientService = chatClientService;
        _serviceProvider = serviceProvider;

        Sessions.CollectionChanged += Sessions_CollectionChanged;

        messenger.Register<PrivateMessageReceivedMessage>(this);
        messenger.Register<GroupMessageReceivedMessage>(this);

        BindingOperations.EnableCollectionSynchronization(Sessions, Sessions);
    }

    [ObservableProperty]
    private Page? _messagesPage;

    [ObservableProperty]
    private IChatSession? _selectedSession;

    public ObservableCollection<IChatSession> Sessions { get; } = new();

    public PrivateChatSession? GetPrivateSession(int userId)
    {
        return Sessions
            .OfType<PrivateChatSession>()
            .FirstOrDefault(session => session.Subject.Id == userId);
    }

    public GroupChatSession? GetGroupSession(int groupId)
    {
        return Sessions
            .OfType<GroupChatSession>()
            .FirstOrDefault(session => session.Subject.Id == groupId);
    }

    public IChatSession? GetSession(IIdentifiable identifiable)
    {
        if (identifiable is User user)
        {
            return GetPrivateSession(user.Id);
        }
        else if (identifiable is Group group)
        {
            return GetGroupSession(group.Id);
        }

        return null;
    }

    public async Task<PrivateChatSession> AddPrivateSessionAsync(int userId)
    {
        var user = await _chatClientService.Client.GetUserAsync(userId);
        var newSession = new PrivateChatSession(user);
        Sessions.Add(newSession);
        return newSession;
    }

    public async Task<GroupChatSession> AddGroupSessionAsync(int groupId)
    {
        var group =await  _chatClientService.Client.GetGroupAsync(groupId);
        var newSession = new GroupChatSession(group);
        Sessions.Add(newSession);
        return newSession;
    }

    public async Task<IChatSession> AddSessionAsync(IIdentifiable identifiable)
    {
        if (identifiable is User user)
        {
            return await AddPrivateSessionAsync(user.Id);
        }
        else if (identifiable is Group group)
        {
            return await AddGroupSessionAsync(group.Id);
        }

        throw new ArgumentException("Invalid value", nameof(identifiable));
    }

    partial void OnSelectedSessionChanged(IChatSession? value)
    {
        if (value is null)
        {
            MessagesPage = null;
            return;
        }

        MessagesPage = _cachedMessagesPages[value];

        // 已读消息
        if (value is PrivateChatSession user)
        {
            user.UnreadMessageCount = 0;
            _ = _chatClientService.Client.SetFriendMessageLastTime(user.Subject.Id, DateTimeOffset.Now);
        }
        else if (value is GroupChatSession group)
        {
            group.UnreadMessageCount = 0;
            _ = _chatClientService.Client.SetGroupMessageLastTime(group.Subject.Id, DateTimeOffset.Now);
        }
    }

    private void Sessions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if ((e.Action == NotifyCollectionChangedAction.Add ||
            e.Action == NotifyCollectionChangedAction.Replace) &&
            e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is not IChatSession session)
                {
                    continue;
                }

                if (!_cachedMessagesPages.TryGetValue(session, out var messagesPage))
                {
                    using var scope = _serviceProvider.CreateScope();

                    if (session is PrivateChatSession user)
                    {
                        var newPage = scope.ServiceProvider.GetRequiredService<PrivateMessagesPage>();
                        newPage.ViewModel.Session = user;

                        messagesPage = newPage;
                    }
                    else if (session is GroupChatSession group)
                    {
                        var newPage = scope.ServiceProvider.GetRequiredService<GroupMessagesPage>();
                        newPage.ViewModel.Session = group;

                        messagesPage = newPage;
                    }
                    else
                    {
                        throw new InvalidOperationException("This would never happen");
                    }

                    _cachedMessagesPages[session] = messagesPage;
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove &&
            e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is not IChatSession session)
                {
                    continue;
                }

                _cachedMessagesPages.Remove(session);
            }
        }
    }

    async void IRecipient<PrivateMessageReceivedMessage>.Receive(PrivateMessageReceivedMessage message)
    {
        var session = GetPrivateSession(message.Message.SenderId);
        if (session is null)
            return;

        await _chatClientService.Client.SetFriendMessageLastTime(message.Message.SenderId, DateTimeOffset.Now);

        session.UnreadMessageCount++;
    }

    async void IRecipient<GroupMessageReceivedMessage>.Receive(GroupMessageReceivedMessage message)
    {
        var session = GetGroupSession(message.Message.GroupId);
        if (session is null)
            return;

        session.UnreadMessageCount++;

        await _chatClientService.Client.SetGroupMessageLastTime(message.Message.GroupId, DateTimeOffset.Now);
    }
}

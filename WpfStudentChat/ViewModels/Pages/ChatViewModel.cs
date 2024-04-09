using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using WpfStudentChat.Models;
using WpfStudentChat.Services;
using WpfStudentChat.Views.Pages;

namespace WpfStudentChat.ViewModels.Pages
{
    public partial class ChatViewModel : ObservableObject
    {
        private readonly ChatClientService _chatClientService;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<IChatSession, Page> _cachedMessagesPages = new();

        public ChatViewModel(
            ChatClientService chatClientService,
            IServiceProvider serviceProvider)
        {
            _chatClientService = chatClientService;
            _serviceProvider = serviceProvider;

            Sessions.CollectionChanged += Sessions_CollectionChanged;
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
                return;

            MessagesPage = _cachedMessagesPages[value];
        }

        private void Sessions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add &&
                e.Action != NotifyCollectionChangedAction.Replace)
            {
                return;
            }

            if (e.NewItems is null)
            {
                return;
            }

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
    }
}

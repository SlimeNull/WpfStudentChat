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
using WpfStudentChat.Services;
using WpfStudentChat.Views.Pages;

namespace WpfStudentChat.ViewModels.Pages
{
    public partial class ChatViewModel : ObservableObject
    {
        private readonly ChatClientService _chatClientService;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<IIdentifiable, Page> _cachedMessagesPages = new();

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
        private IIdentifiable? _selectedSession;

        public ObservableCollection<IIdentifiable> Sessions { get; } = new();

        partial void OnSelectedSessionChanged(IIdentifiable? value)
        {
            if (value is null)
            {
                return;
            }

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

            foreach (var session in e.NewItems)
            {
                if (session is not IIdentifiable identifiable)
                {
                    continue;
                }

                if (!_cachedMessagesPages.TryGetValue(identifiable, out var messagesPage))
                {
                    using var scope = _serviceProvider.CreateScope();

                    if (identifiable is User user)
                    {
                        var newPage = scope.ServiceProvider.GetRequiredService<PrivateMessagesPage>();
                        newPage.ViewModel.Target = user;

                        messagesPage = newPage;
                    }
                    else if (identifiable is Group group)
                    {
                        var newPage = scope.ServiceProvider.GetRequiredService<GroupMessagesPage>();
                        newPage.ViewModel.Target = group;

                        messagesPage = newPage;
                    }
                    else
                    {
                        throw new InvalidOperationException("This would never happen");
                    }

                    _cachedMessagesPages[identifiable] = messagesPage;
                }
            }
        }
    }
}

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
        public ChatPage(
            ChatViewModel viewModel,
            ChatClientService chatClientService,
            IMessenger messenger)
        {
            SelfUserId = chatClientService.Client.GetSelfUserId();
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            messenger.Register<PrivateMessageReceivedMessage>(this);
            messenger.Register<GroupMessageReceivedMessage>(this);
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
            if (ViewModel.GetGroupSession(message.Message.SenderId) is null)
            {
                var newSession = await ViewModel.AddGroupSessionAsync(message.Message.GroupId);
                newSession.Messages.Add(message.Message);
            }
        }
    }
}

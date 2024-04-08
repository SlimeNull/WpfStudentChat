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
using WpfStudentChat.Models.Messages;
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
            IMessenger messenger)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            messenger.Register<PrivateMessageReceivedMessage>(this);
            messenger.Register<GroupMessageReceivedMessage>(this);
        }

        public ChatViewModel ViewModel { get; }

        public void EnsureSession(IIdentifiable identifiable)
        {
            if (identifiable is User)
            {
                if (!ViewModel.Sessions.OfType<User>().Any(session => session.Id == identifiable.Id))
                {
                    ViewModel.Sessions.Add(identifiable);
                }
            }
            else if (identifiable is Group)
            {
                if (!ViewModel.Sessions.OfType<Group>().Any(session => session.Id == identifiable.Id))
                {
                    ViewModel.Sessions.Add(identifiable);
                }
            }
        }

        public void SelectSession(IIdentifiable identifiable)
        {
            ViewModel.SelectedSession = identifiable;
        }

        void IRecipient<PrivateMessageReceivedMessage>.Receive(PrivateMessageReceivedMessage message)
        {
            
        }

        void IRecipient<GroupMessageReceivedMessage>.Receive(GroupMessageReceivedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

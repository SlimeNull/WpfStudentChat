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
using WpfStudentChat.Models.Messages;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages
{
    /// <summary>
    /// GroupMessagesPage.xaml 的交互逻辑
    /// </summary>
    public partial class GroupMessagesPage : Page, IRecipient<GroupMessageReceivedMessage>
    {
        public GroupMessagesPage(
            GroupMessagesViewModel viewModel,
            IMessenger messenger)
        {

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            messenger.Register<GroupMessageReceivedMessage>(this);
        }

        public GroupMessagesViewModel ViewModel { get; }

        void IRecipient<GroupMessageReceivedMessage>.Receive(GroupMessageReceivedMessage message)
        {
            ViewModel.Messages.Add(message.Message);
        }
    }
}

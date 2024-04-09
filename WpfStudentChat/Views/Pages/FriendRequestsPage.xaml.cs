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
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages
{
    /// <summary>
    /// FriendRequestsPage.xaml 的交互逻辑
    /// </summary>
    public partial class FriendRequestsPage : Page,
        IRecipient<FriendRequestReceivedMessage>
    {
        const int LoadCount = 20;

        private readonly ChatClientService _chatClientService;

        public FriendRequestsPage(
            FriendRequestsViewModel viewModel,
            ChatClientService chatClientService)
        {
            _chatClientService = chatClientService;
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        public FriendRequestsViewModel ViewModel { get; }

        [RelayCommand]
        public async Task LoadRequests()
        {
            try
            {
                var requests = await _chatClientService.Client.GetFriendRequestsAsync(0, LoadCount);
                ViewModel.Requests.Clear();
                foreach (var request in requests)
                    ViewModel.Requests.Add(request);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Failed to load requests. {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        public async Task LoadMoreRequests()
        {
            try
            {
                var requests = await _chatClientService.Client.GetFriendRequestsAsync(ViewModel.Requests.Count, LoadCount);
                foreach (var request in requests)
                    ViewModel.Requests.Add(request);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, $"Failed to load more requests. {ex.Message}", "Error");
            }
        }

        void IRecipient<FriendRequestReceivedMessage>.Receive(FriendRequestReceivedMessage message)
        {
            ViewModel.Requests.Insert(0, message.Request);
        }
    }
}

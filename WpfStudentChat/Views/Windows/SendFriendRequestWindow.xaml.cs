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
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows
{
    /// <summary>
    /// SendFriendRequestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SendFriendRequestWindow : UiWindow
    {
        private readonly ChatClientService _chatClientService;

        public SendFriendRequestWindow(
            SendFriendRequestViewModel viewModel,
            ChatClientService chatClientService)
        {
            _chatClientService = chatClientService;

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public SendFriendRequestViewModel ViewModel { get; }

        [RelayCommand]
        public async Task Send()
        {
            try
            {
                await _chatClientService.Client.SendFriendRequestAsync(ViewModel.Profile.Id, ViewModel.Message);
                Close();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(this, $"发送好友请求失败. {ex.Message}");
            }
        }
    }
}

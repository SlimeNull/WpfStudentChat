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
using StudentChat.Models;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows
{
    /// <summary>
    /// SearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWindow : UiWindow
    {
        const int SearchCount = 30;

        private readonly ChatClientService _chatClientService;

        public SearchWindow(
            SearchViewModel viewModel,
            ChatClientService chatClientService)
        {
            _chatClientService = chatClientService;

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public SearchViewModel ViewModel { get; }

        [RelayCommand]
        public async Task SearchUsers()
        {
            ViewModel.CurrentSearchUserKeyword = ViewModel.SearchUserKeyword;
            ViewModel.HasMoreUsers = true;

            ViewModel.SearchUserResults.Clear();
            var users = await _chatClientService.Client.SearchUserAsync(ViewModel.CurrentSearchUserKeyword, 0, SearchCount);

            foreach (var user in users)
            {
                ViewModel.SearchUserResults.Add(user);
            }

            ViewModel.HasMoreUsers = users.Count == SearchCount;
        }

        [RelayCommand]
        public async Task SearchGroups()
        {
            ViewModel.CurrentSearchGroupKeyword = ViewModel.SearchGroupKeyword;
            ViewModel.HasMoreGroups = true;

            ViewModel.SearchGroupResults.Clear();
            var groups = await _chatClientService.Client.SearchGroupAsync(ViewModel.CurrentSearchGroupKeyword, 0, SearchCount);

            foreach (var group in groups)
            {
                ViewModel.SearchGroupResults.Add(group);
            }

            ViewModel.HasMoreGroups = groups.Count == SearchCount;
        }

        [RelayCommand]
        public async Task LoadMoreUsers()
        {
            var users = await _chatClientService.Client.SearchUserAsync(ViewModel.CurrentSearchUserKeyword, ViewModel.SearchUserResults.Count, SearchCount);

            foreach (var user in users)
            {
                ViewModel.SearchUserResults.Add(user);
            }

            ViewModel.HasMoreUsers = users.Count == SearchCount;
        }

        [RelayCommand]
        public async Task LoadMoreGroups()
        {
            var groups = await _chatClientService.Client.SearchGroupAsync(ViewModel.CurrentSearchGroupKeyword, ViewModel.SearchGroupResults.Count, SearchCount);

            foreach (var group in groups)
            {
                ViewModel.SearchGroupResults.Add(group);
            }

            ViewModel.HasMoreGroups = groups.Count == SearchCount;
        }

        [RelayCommand]
        public async Task SendFriendRequest(User user)
        {

        }

        [RelayCommand]
        public async Task SendGroupRequest(Group group)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using StudentChat.Models;
using WpfStudentChat.Extensions;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;
using WpfStudentChat.ViewModels.Windows;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Views.Pages
{
    /// <summary>
    /// PrivateMessagesPage.xaml 的交互逻辑
    /// </summary>
    public partial class PrivateMessagesPage : Page, IRecipient<PrivateMessageReceivedMessage>
    {
        private readonly ChatClientService _chatClientService;

        public PrivateMessagesPage(
            PrivateMessagesViewModel viewModel,
            ChatClientService chatClientService,
            IMessenger messenger)
        {
            _chatClientService = chatClientService;
            SelfUserId = _chatClientService.Client.GetSelfUserId();
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            messenger.Register(this);
        }

        public int SelfUserId { get; }
        public PrivateMessagesViewModel ViewModel { get; }

        void IRecipient<PrivateMessageReceivedMessage>.Receive(PrivateMessageReceivedMessage message)
        {
            if (ViewModel.Session is null)
                return;

            var atBottom = MessagesScrollViewer.IsAtBottom();
            ViewModel.Session.Messages.Add(message.Message);

            if (atBottom)
                MessagesScrollViewer.ScrollToBottom();
        }

        [RelayCommand]
        public async Task SendMessageAsync()
        {
            string textInput = ViewModel.TextInput;

            try
            {
                ViewModel.TextInput = string.Empty;
                if (ViewModel.Session is null)
                {
                    return;
                }

                await _chatClientService.Client.SendPrivateMessageAsync(ViewModel.Session.Subject.Id, textInput, null, null);
            }
            catch (Exception ex)
            {
                ViewModel.TextInput = textInput;
                MessageBox.Show(App.Current.MainWindow, $"Failed to send message. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void OpenSendImageWindow()
        {
            if (ViewModel.Session is null)
                return;

            using var scope = App.Host.Services.CreateScope();
            var window = scope.ServiceProvider.GetRequiredService<SendImageWindow>();
            window.Owner = App.Current.MainWindow;
            window.ViewModel.Target = ViewModel.Session.Subject;

            window.ShowDialog();
        }

        [RelayCommand]
        public async void SaveAttachment(Attachment attachment)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                FileName = attachment.Name,
                Filter = "Any file|*.*",
                CheckPathExists = true,
            };

            if (sfd.ShowDialog() ?? false)
            {
                try
                {
                    using var attachmentStream = await _chatClientService.Client.GetImageAsync(attachment.AttachmentHash);
                    using var fileStream = File.Create(sfd.FileName);

                    await attachmentStream.CopyToAsync(fileStream);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(App.Current.MainWindow, $"保存失败. {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

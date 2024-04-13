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
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Views.Pages
{
    /// <summary>
    /// GroupMessagesPage.xaml 的交互逻辑
    /// </summary>
    public partial class GroupMessagesPage : Page, IRecipient<GroupMessageReceivedMessage>
    {
        private readonly ChatClientService _chatClientService;

        public GroupMessagesPage(
            GroupMessagesViewModel viewModel,
            ChatClientService chatClientService,
            IMessenger messenger)
        {

            ViewModel = viewModel;
            _chatClientService = chatClientService;
            DataContext = this;

            InitializeComponent();

            messenger.Register<GroupMessageReceivedMessage>(this);
        }

        public GroupMessagesViewModel ViewModel { get; }

        void IRecipient<GroupMessageReceivedMessage>.Receive(GroupMessageReceivedMessage message)
        {
            if (ViewModel.Session is null)
                return;

            bool atBottom = MessagesScrollViewer.IsAtBottom();
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

                await _chatClientService.Client.SendGroupMessageAsync(ViewModel.Session.Subject.Id, textInput, null, null);
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
        public async void OpenSendFileDialog()
        {
            if (ViewModel.Session is null)
                return;

            var ofd = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "Any file|*.*"
            };

            if (ofd.ShowDialog() ?? false)
            {
                try
                {
                    var fileName = System.IO.Path.GetFileName(ofd.FileName);
                    var fileStream = File.OpenRead(ofd.FileName);
                    string attachmentHash = await _chatClientService.Client.UploadFileAsync(fileStream, fileName);
                    var fileAttachment = new Attachment()
                    {
                        Name = fileName,
                        AttachmentHash = attachmentHash
                    };

                    await _chatClientService.Client.SendGroupMessageAsync(ViewModel.Session.Subject.Id, string.Empty, null, new Attachment[] { fileAttachment });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(App.Current.MainWindow, $"发送失败. {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                catch (Exception ex)
                {
                    MessageBox.Show(App.Current.MainWindow, $"保存失败. {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

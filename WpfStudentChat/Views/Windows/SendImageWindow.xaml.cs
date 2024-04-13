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
using System.Windows.Shapes;
using Microsoft.Win32;
using StudentChat.Models;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows
{
    /// <summary>
    /// SendImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SendImageWindow : Window
    {
        private readonly ChatClientService _chatClientService;

        public SendImageWindow(
            SendImageViewModel viewModel,
            ChatClientService chatClientService)
        {
            _chatClientService = chatClientService;

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public SendImageViewModel ViewModel { get; }

        [RelayCommand]
        public void SelectImage()
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Image file|*.jpg;*.jpeg;*.png;*.bmp",
                CheckFileExists = true,
            };

            if (ofd.ShowDialog() ?? false)
            {
                ViewModel.ImagePath = ofd.FileName;
            }
        }

        [RelayCommand]
        public async Task Send()
        {
            if (ViewModel.Target is null)
            {
                return;
            }

            if (!File.Exists(ViewModel.ImagePath))
            {
                MessageBox.Show(this, "请选择图片", "提示");
                return;
            }

            var fileStream = File.OpenRead(ViewModel.ImagePath);
            var fileName = System.IO.Path.GetFileName(ViewModel.ImagePath);

            try
            {
                var imageHash = await _chatClientService.Client.UploadImageAsync(fileStream, fileName);
                var imageAttachment = new Attachment()
                {
                    Name = fileName,
                    AttachmentHash = imageHash,
                };

                if (ViewModel.Target is User userTarget)
                {
                    await _chatClientService.Client.SendPrivateMessageAsync(userTarget.Id, ViewModel.Caption, new Attachment[] { imageAttachment }, null);
                    Close();
                }
                else if (ViewModel.Target is Group groupTarget)
                {
                    await _chatClientService.Client.SendGroupMessageAsync(groupTarget.Id, ViewModel.Caption, new Attachment[] { imageAttachment }, null);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"发送失败. {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}

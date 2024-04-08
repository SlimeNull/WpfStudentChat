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
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows
{
    /// <summary>
    /// SetProfileWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetProfileWindow : FluentWindow
    {
        private readonly ChatClientService _chatClientService;

        public SetProfileWindow(
            SetProfileViewModel viewModel,
            ChatClientService chatClientService)
        {
            _chatClientService = chatClientService;

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public SetProfileViewModel ViewModel { get; }

        [RelayCommand]
        public async Task LoadProfile()
        {
            try
            {
                ViewModel.Profile = await _chatClientService.Client.GetSelf();
            }
            catch
            {
                System.Windows.MessageBox.Show(this, "Failed to load profile", "Error");
            }
        }

        [RelayCommand]
        public async Task UploadImage()
        {
            try
            {
                var ofd = new OpenFileDialog()
                {
                    Filter = "Image file|*.jpg;*.jpeg;*.png",
                    Multiselect = false,
                    CheckFileExists = true,
                };

                var result = ofd.ShowDialog(this) ?? false;

                if (!result)
                    return;

                var stream = File.OpenRead(ofd.FileName);
                var fileName = System.IO.Path.GetFileName(ofd.FileName);
                var hash = await _chatClientService.Client.UploadImageAsync(stream, fileName);

                if (ViewModel.Profile is null)
                {
                    ViewModel.Profile = new StudentChat.Models.User();
                }

                ViewModel.Profile = ViewModel.Profile with
                {
                    AvatarHash = hash
                };
            }
            catch
            {
                System.Windows.MessageBox.Show(this, "Failed to upload image", "Error");
            }
        }

        [RelayCommand]
        public async Task SaveProfileAndClose()
        {
            try
            {
                await _chatClientService.Client.SetSelf(ViewModel.Profile);
                Hide();
            }
            catch
            {
                System.Windows.MessageBox.Show(this, "Failed to save profile", "Error");
            }
        }

        private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}

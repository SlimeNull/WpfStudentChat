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
    /// CreateGroupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetGroupProfileWindow : UiWindow
    {
        private readonly ChatClientService _chatClientService;

        public SetGroupProfileWindow(
            SetGroupProfileViewModel viewModel,
            ChatClientService chatClientService)
        {
            _chatClientService = chatClientService;

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public SetGroupProfileViewModel ViewModel { get; }

        [RelayCommand]
        public async Task LoadProfile()
        {
            try
            {
                if (ViewModel.Profile.Id != 0)
                {
                    ViewModel.Profile = await _chatClientService.Client.GetGroupAsync(ViewModel.Profile.Id);
                }
            }
            catch
            {

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
                    ViewModel.Profile = new StudentChat.Models.Group();
                }

                ViewModel.Profile = ViewModel.Profile with
                {
                    AvatarHash = hash
                };
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(this, $"Failed to upload image. {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        public async Task SaveProfileAndClose()
        {
            try
            {
                if (ViewModel.Profile.Id != 0)
                {
                    await _chatClientService.Client.SetGroupAsync(ViewModel.Profile);
                }
                else
                {
                    await _chatClientService.Client.CreateGroupAsync(ViewModel.Profile);
                }

                Close();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(this, $"Failed to save group profile. {ex.Message}", "Error");
            }
        }
    }
}

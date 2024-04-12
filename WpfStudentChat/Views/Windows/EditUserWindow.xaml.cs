using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using StudentChat.Models;
using WpfStudentChat.Services;

namespace WpfStudentChat.Views.Windows;

/// <summary>
/// Interaction logic for EditUserWindow.xaml
/// </summary>
public partial class EditUserWindow : Window, INotifyPropertyChanged
{
    private readonly ChatClientService _chatClientService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public User? User { get; set; } // clone

    public string? UserPassword { get; set; }
    public Stream? AvatarStream { get; set; }
    public string? AvatarName { get; set; }

    public EditUserWindow(ChatClientService chatClientService)
    {
        DataContext = this;
        InitializeComponent();
        _chatClientService = chatClientService;
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    [RelayCommand]
    public async Task Save()
    {
        if (User is null)
            return;

        await _chatClientService.Client.UpdateUserInfo(User, UserPassword);
        DialogResult = true;
        Close();
    }

    [RelayCommand]
    public void SetPassword()
    {
        var result = Microsoft.VisualBasic.Interaction.InputBox("输入密码", "");
        if (!string.IsNullOrEmpty(result))
        {
            UserPassword = result;
        }
    }

    [RelayCommand]
    public async void SetImage()
    {
        if (User is null)
            return;

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

        AvatarStream = stream;
        AvatarName = fileName;

        try
        {
            User.AvatarHash = await _chatClientService.Client.UploadImageAsync(AvatarStream, AvatarName);
            OnPropertyChanged(nameof(User));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to upload image. {ex.Message}", "Error");
            return;
        }
    }

    [RelayCommand]
    private void OnClose() => Close();
}

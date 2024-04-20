using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
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

    public string? UserPasswordHash { get; set; }
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

        await _chatClientService.Client.UpdateUserInfo(User, UserPasswordHash);
        DialogResult = true;
        Close();
    }

    [RelayCommand]
    public void ResetPassword()
    {
        byte[] passwordHash = SHA256.HashData("1234"u8);
        string passwordHashText = Convert.ToHexString(passwordHash);
        UserPasswordHash = passwordHashText; // 重置密码成1234
    }

    [RelayCommand]
    public async Task SetImage()
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
            MessageBox.Show(this, $"上传头像失败. {ex.Message}", "错误");
            return;
        }
    }

    [RelayCommand]
    private void OnClose() => Close();
}

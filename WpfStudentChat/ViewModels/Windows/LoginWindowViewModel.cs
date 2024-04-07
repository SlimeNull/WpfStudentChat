﻿using CommunityToolkit.Mvvm.Messaging;
using StudentChat;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;

namespace WpfStudentChat.ViewModels.Windows;

public partial class LoginWindowViewModel(ChatClientService chatClientService, IMessenger messenger) : ObservableObject
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    public bool IsLogged { get; set; }

    [RelayCommand]
    public async Task Login()
    {
        try
        {
            await chatClientService.Client.LoginAsync(Username, Password);

            IsLogged = true;
            messenger.Send(new LoggedMessage());
        }
        catch (Exception)
        {
            MessageBox.Show("登录失败");
            return;
        }

        await Task.CompletedTask;
    }
}

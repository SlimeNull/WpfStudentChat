﻿using CommunityToolkit.Mvvm.Messaging;
using StudentChat;
using WpfStudentChat.Services;

namespace WpfStudentChat.ViewModels.Windows;

public partial class LoginWindowViewModel : ObservableObject
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    public LoginWindowViewModel()
    {
#if DEBUG
        Task.Delay(500).ContinueWith(task =>
        {
            Username = "Test";
            Password = "TestHash";
        });
#endif
    }
}

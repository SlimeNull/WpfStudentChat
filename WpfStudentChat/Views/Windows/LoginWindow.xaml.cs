using CommunityToolkit.Mvvm.Messaging;
using Wpf.Ui.Controls;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : FluentWindow, IRecipient<LoggedMessage>
{
    private readonly IMessenger _messenger;

    public LoginWindowViewModel ViewModel { get; }

    public LoginWindow(LoginWindowViewModel loginWindowViewModel, IMessenger messenger)
    {
        ViewModel = loginWindowViewModel;
        _messenger = messenger;
        DataContext = this;
        InitializeComponent();

        messenger.Register<LoggedMessage>(this);
    }

    public void Receive(LoggedMessage message)
    {
        Close();
    }
}

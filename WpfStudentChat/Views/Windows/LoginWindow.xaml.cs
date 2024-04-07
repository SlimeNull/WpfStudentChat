using CommunityToolkit.Mvvm.Messaging;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window, IRecipient<LoggedMessage>
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

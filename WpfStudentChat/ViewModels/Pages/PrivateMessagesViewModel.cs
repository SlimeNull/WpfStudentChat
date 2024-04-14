using WpfStudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages;

public partial class PrivateMessagesViewModel : ObservableObject
{
    [ObservableProperty]
    private string _textInput = string.Empty;

    [ObservableProperty]
    private PrivateChatSession? _session;
}

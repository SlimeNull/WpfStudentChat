using System.Collections.ObjectModel;
using StudentChat.Models;
using WpfStudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages
{
    public partial class GroupMessagesViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _textInput = string.Empty;

        [ObservableProperty]
        private GroupChatSession? _session;
    }
}

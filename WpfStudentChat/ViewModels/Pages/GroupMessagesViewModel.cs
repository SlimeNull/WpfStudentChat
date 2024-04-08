using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages
{
    public partial class GroupMessagesViewModel : ObservableObject
    {
        [ObservableProperty]
        private Group? _target;

        [ObservableProperty]
        private string _textInput = string.Empty;

        public ObservableCollection<GroupMessage> Messages { get; } = new();
    }
}

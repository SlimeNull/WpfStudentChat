using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows;

public partial class SendGroupRequestViewModel : ObservableObject
{
    [ObservableProperty]
    private Group _profile = new();

    [ObservableProperty]
    private string _message = string.Empty;
}

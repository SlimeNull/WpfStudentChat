using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows;

public partial class SetProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private User profile = new();
}

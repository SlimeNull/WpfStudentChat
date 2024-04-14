using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows;

public partial class SetGroupProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private Group profile = new();
}

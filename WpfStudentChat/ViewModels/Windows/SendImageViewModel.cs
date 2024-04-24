using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows;

public partial class SendImageViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _imagePath;

    [ObservableProperty]
    private string _caption = string.Empty;

    [ObservableProperty]
    private IIdentifiable? _target;
}

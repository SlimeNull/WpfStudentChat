using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages;

public partial class ContactsViewModel : ObservableObject
{
    [ObservableProperty] private object? _selectedItem;

    public ObservableCollection<User> Friends { get; } = new();
    public ObservableCollection<Group> Groups { get; } = new();

}

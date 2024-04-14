using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Windows;

public partial class SearchViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchUserKeyword = string.Empty;

    [ObservableProperty]
    private string _searchGroupKeyword = string.Empty;

    [ObservableProperty]
    private string _currentSearchUserKeyword = string.Empty;

    [ObservableProperty]
    private string _currentSearchGroupKeyword = string.Empty;

    [ObservableProperty]
    private bool _hasMoreUsers = false;

    [ObservableProperty]
    private bool _hasMoreGroups = false;

    public ObservableCollection<User> SearchUserResults { get; } = new();
    public ObservableCollection<Group> SearchGroupResults { get; } = new();
}

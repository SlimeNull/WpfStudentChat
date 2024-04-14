using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages;

public partial class FriendRequestsViewModel : ObservableObject
{
    public ObservableCollection<FriendRequest> Requests { get; } = new();
}

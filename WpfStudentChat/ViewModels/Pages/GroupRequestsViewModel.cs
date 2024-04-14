using System.Collections.ObjectModel;
using StudentChat.Models;

namespace WpfStudentChat.ViewModels.Pages;

public partial class GroupRequestsViewModel : ObservableObject
{
    public ObservableCollection<GroupRequest> Requests { get; } = new();
    
}

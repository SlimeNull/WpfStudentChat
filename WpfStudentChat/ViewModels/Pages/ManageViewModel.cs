using StudentChat.Models;
using WpfStudentChat.Services;

namespace WpfStudentChat.ViewModels.Pages;

public partial class ManageViewModel : ObservableObject
{
    private readonly ChatClientService _chatClientService;
    [ObservableProperty] private IEnumerable<User> _users = [];

    public ManageViewModel(ChatClientService chatClientService)
    {
        _chatClientService = chatClientService;
    }

    [RelayCommand]
    public async Task LoadUsers()
    {
        var users = await _chatClientService.Client.GetUsers("", "", 0, 1000000);

        Users = users.Users;
    }
}

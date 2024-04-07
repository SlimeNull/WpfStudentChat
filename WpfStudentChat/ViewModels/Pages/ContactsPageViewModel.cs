using System.Collections.ObjectModel;
using StudentChat.Models;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.Views.Pages;

namespace WpfStudentChat.ViewModels.Pages;

public partial class ContactsPageViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty] private object _selectedItem;
    private readonly ChatClientService _chatClientService;

    [ObservableProperty] private object? _content = null;
    [ObservableProperty] private object? _friendGroupRequestIndex = null; // 0: friend request, 1: group request

    public ObservableCollection<IIdentifiable> Identifiables { get; } = new();

    public ContactsPageViewModel(ChatClientService chatClientService)
    {
#if true
        _selectedItem = null!;
        Identifiables = [new Group() { Name = "balaba" }, new Group() { Name = "诺尔" }];
        _chatClientService = chatClientService;
#endif
    }



    partial void OnSelectedItemChanged(object value)
    {
        if (value is User user)
        {
            Content = new ContactsFriendPage(user);
        }
        else if (value is Group group)
        {
            Content = new ContactsGroupPage(group);
        }
    }

    partial void OnFriendGroupRequestIndexChanged(object? value)
    {
        if (FriendGroupRequestIndex is 0)
        {
            Content = "好友请求";
        }
        else if(FriendGroupRequestIndex is 1)
        {
            Content = "群请求";
        }
    }

    public void OnNavigatedTo()
    {
        // UserIdentifiables = _chatClientService.Client.GetGroups();
        // GroupIdentifiables = _chatClientService.Client.GetUsers();
    }

    public void OnNavigatedFrom() { }

    [RelayCommand]
    public void SwitchFriend()
    {
        //Identifiables = UserIdentifiables;
    }

    [RelayCommand]
    public void SwitchGroup()
    {
        //Identifiables = GroupIdentifiables;
    }
}

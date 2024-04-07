using System.Windows.Controls;
using StudentChat.Models;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsFriendPage.xaml
/// </summary>
public partial class ContactsFriendPage : Page
{
    public User User { get; }
    public ContactsFriendPage(User user)
    {
        User = user;
        DataContext = this;
        InitializeComponent();
    }
}

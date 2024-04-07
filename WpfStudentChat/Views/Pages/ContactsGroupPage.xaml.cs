using System.Windows.Controls;
using StudentChat.Models;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsGroupPage.xaml
/// </summary>
public partial class ContactsGroupPage : Page
{
    public Group Group { get; }
    public ContactsGroupPage(Group group)
    {
        Group = group;
        DataContext = this;
        InitializeComponent();
    }
}

using System.Windows.Controls;
using Wpf.Ui.Controls;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ContactsPage.xaml
/// </summary>
public partial class ContactsPage : Page, INavigableView<ContactsPageViewModel>
{
    public ContactsPageViewModel ViewModel { get; }
    public ContactsPage(ContactsPageViewModel contactsPageViewModel)
    {
        ViewModel = contactsPageViewModel;
        DataContext = this;
        InitializeComponent();
    }
}

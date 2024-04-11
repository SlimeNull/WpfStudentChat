using System.Windows.Controls;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for ManagePage.xaml
/// </summary>
public partial class ManagePage : Page
{
    public ManageViewModel ViewModel { get; }

    public ManagePage(ManageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
        Loaded += ManagePage_Loaded;
    }

    private async void ManagePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadUsers();
    }
}

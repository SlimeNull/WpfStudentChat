using System.Collections.ObjectModel;
using StudentChat.Models;
using Wpf.Ui.Controls;

namespace WpfStudentChat.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _applicationTitle = "WPF UI - WpfStudentChat";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem()
        {
            Content = "消息",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(Views.Pages.ChatPage)
        },
        new NavigationViewItem()
        {
            Content = "联系人",
            Icon = new SymbolIcon { Symbol = SymbolRegular.ContactCard24 },
            TargetPageType = typeof(Views.Pages.ContactsPage)
        },
        new NavigationViewItem()
        {
            Content = "Data",
            Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
            TargetPageType = typeof(Views.Pages.DataPage)
        },
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem()
        {
            Content = "设置",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(Views.Pages.SettingsPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "Home", Tag = "tray_home" }
    };

    [ObservableProperty]
    private User? _profile;
}

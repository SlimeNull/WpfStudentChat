using System.Collections.ObjectModel;
using StudentChat.Models;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace WpfStudentChat.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _applicationTitle = "WPF UI - WpfStudentChat";

    [ObservableProperty]
    private ObservableCollection<INavigationControl> _menuItems = new()
    {
        new NavigationItem()
        {
            Content = "消息",
            Icon = SymbolRegular.Home24,
            PageType = typeof(Views.Pages.ChatPage)
        },
        new NavigationItem()
        {
            Content = "联系人",
            Icon = SymbolRegular.ContactCard24,
            PageType = typeof(Views.Pages.ContactsPage)
        },
        new NavigationItem()
        {
            Content = "Data",
            Icon = SymbolRegular.DataHistogram24,
            PageType = typeof(Views.Pages.DataPage)
        },
    };

    [ObservableProperty]
    private ObservableCollection<INavigationControl> _footerMenuItems = new()
    {
        new NavigationItem()
        {
            Content = "设置",
            Icon = SymbolRegular.Settings24,
            PageType = typeof(Views.Pages.SettingsPage)
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

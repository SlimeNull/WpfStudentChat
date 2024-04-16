using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using StudentChat.Models;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace WpfStudentChat.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _applicationTitle = "聊天";

    [ObservableProperty]
    private ObservableCollection<INavigationControl> _menuItems = [];

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

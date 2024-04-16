using System.Diagnostics;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using WpfStudentChat.ViewModels.Pages;

namespace WpfStudentChat.Views.Pages;

public partial class SettingsPage : INavigableView<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    [RelayCommand]
    public void Logout()
    {
        Process.Start(Environment.ProcessPath!);
        Environment.Exit(0);
    }
}

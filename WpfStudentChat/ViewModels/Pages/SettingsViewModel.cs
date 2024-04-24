using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;
using WpfStudentChat.Services;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private string _appVersion = String.Empty;

    [ObservableProperty]
    private ThemeType _currentTheme = ThemeType.Unknown;
    private readonly ChatClientService _chatClientService;

    public SettingsViewModel(ChatClientService chatClientService)
    {
        _chatClientService = chatClientService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom() { }

    private void InitializeViewModel()
    {
        CurrentTheme = Theme.GetAppTheme();
        AppVersion = $"设置 - {GetAssemblyVersion()}";

        _isInitialized = true;
    }

    private string GetAssemblyVersion()
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
            ?? String.Empty;
    }

    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentTheme == ThemeType.Light)
                    break;

                Theme.Apply(ThemeType.Light);
                CurrentTheme = ThemeType.Light;

                break;

            default:
                if (CurrentTheme == ThemeType.Dark)
                    break;

                Theme.Apply(ThemeType.Dark);
                CurrentTheme = ThemeType.Dark;

                break;
        }
    }

    [RelayCommand]
    private void OnShowSetPasswordWindow()
    {
        var window = new SetPasswordWindow(_chatClientService);
        window.Owner = Application.Current.MainWindow;
        window.ShowDialog();
    }
}

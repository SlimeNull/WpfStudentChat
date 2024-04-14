using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;

namespace WpfStudentChat.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized = false;

    [ObservableProperty]
    private string _appVersion = String.Empty;

    [ObservableProperty]
    private ThemeType _currentTheme = ThemeType.Unknown;

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
}

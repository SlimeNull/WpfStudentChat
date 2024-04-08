using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows;

public partial class MainWindow : FluentWindow, INavigationWindow
{
    private readonly ChatClientService _chatClientService;
    private readonly IServiceProvider _serviceProvider;

    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        ChatClientService chatClientService,
        IPageService pageService,
        INavigationService navigationService,
        IServiceProvider serviceProvider
    )
    {
        _chatClientService = chatClientService;
        _serviceProvider = serviceProvider;

        ViewModel = viewModel;
        DataContext = this;

        SystemThemeWatcher.Watch(this);

        InitializeComponent();
        SetPageService(pageService);

        navigationService.SetNavigationControl(RootNavigation);

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {

    }

    #region INavigationWindow methods

    public INavigationView GetNavigation() => RootNavigation;

    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

    public void SetServiceProvider(IServiceProvider serviceProvider) { }

    public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    #endregion INavigationWindow methods

    /// <summary>
    /// Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    [RelayCommand]
    public async Task LoadProfile()
    {
        try
        {
            ViewModel.Profile = await _chatClientService.Client.GetSelf();
        }
        catch
        {
            System.Windows.MessageBox.Show(this, "Failed to load profile", "Error");
        }
    }

    [RelayCommand]
    public void ShowSetProfileWindow()
    {
        var window = _serviceProvider.GetRequiredService<SetProfileWindow>();
        window.Owner = this;
        window.ShowDialog();

        _ = LoadProfileCommand.ExecuteAsync(null);
    }
}

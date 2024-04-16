using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.Mvvm.Contracts;
using WpfStudentChat.Models.Database;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private INavigationWindow? _navigationWindow;

    public ApplicationHostService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await HandleActivationAsync();
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates main window during activation.
    /// </summary>
    private async Task HandleActivationAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        await db.Database.EnsureCreatedAsync();

        var chatClientService = _serviceProvider.GetRequiredService<ChatClientService>();
        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        if (loginWindow.ShowDialog() ?? false)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            _navigationWindow = mainWindow;
            _navigationWindow.ShowWindow();

            if (!chatClientService.Client.IsAdmin)
            {
                _navigationWindow.Navigate(typeof(Views.Pages.ChatPage));
            }
            else
            {
                _navigationWindow.Navigate(typeof(Views.Pages.ManagePage));
            }
        }
        else
        {
            Application.Current.Shutdown();
        }

        await Task.CompletedTask;
    }
}

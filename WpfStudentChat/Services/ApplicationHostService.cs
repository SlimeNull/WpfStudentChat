﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private INavigationWindow _navigationWindow;

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
        if (!Application.Current.Windows.OfType<MainWindow>().Any())
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            Application.Current.MainWindow = mainWindow;

            _navigationWindow = mainWindow;
        }

        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.ShowDialog();

        if (loginWindow.ViewModel.IsLogged)
        {
            _navigationWindow.ShowWindow();
            _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
        }
        else
        {
            Application.Current.Shutdown();
        }

        await Task.CompletedTask;
    }
}

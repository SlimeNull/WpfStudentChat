﻿using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using WpfStudentChat.Models;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;
using WpfStudentChat.ViewModels.Windows;
using WpfStudentChat.Views.Pages;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public static IHost Host { get; } = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => 
        { 
            c.SetBasePath(AppContext.BaseDirectory); 
        })
        .ConfigureServices((context, services) =>
        {
            services.Configure<AppConfig>(context.Configuration);

            services.AddHostedService<ApplicationHostService>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

            // Page resolver service
            services.AddSingleton<IPageService, PageService>();

            // Theme manipulation
            services.AddSingleton<IThemeService, ThemeService>();

            // TaskBar manipulation
            services.AddSingleton<ITaskBarService, TaskBarService>();

            // Service containing navigation, same as INavigationWindow... but without window
            services.AddSingleton<INavigationService, NavigationService>();

            // Main window with navigation
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            // Chat
            services.AddSingleton<ChatClientService>();

            // Pages
            services.AddSingleton<ChatPage>();
            services.AddSingleton<ChatViewModel>();
            services.AddSingleton<DataPage>();
            services.AddSingleton<DataViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<ContactsPage>();
            services.AddSingleton<ContactsViewModel>();

            // Scoped Pages
            services.AddScoped<PrivateMessagesPage>();
            services.AddScoped<PrivateMessagesViewModel>();
            services.AddScoped<GroupMessagesPage>();
            services.AddScoped<GroupMessagesViewModel>();

            // Windows
            services.AddSingleton<LoginWindow>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<SetProfileWindow>();
            services.AddSingleton<SetProfileViewModel>();
        }).Build();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Host.Start();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        
        await Host.StopAsync();
        Host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}

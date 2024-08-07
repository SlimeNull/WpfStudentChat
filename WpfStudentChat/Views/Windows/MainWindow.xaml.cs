﻿using System.Printing;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Windows;

namespace WpfStudentChat.Views.Windows;

public partial class MainWindow : Wpf.Ui.Controls.UiWindow, INavigationWindow
{
    private readonly ChatClientService _chatClientService;
    private readonly IServiceProvider _serviceProvider;

    public bool SelfIsAdmin => _chatClientService.Client.IsAdmin;
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

        Wpf.Ui.Appearance.Watcher.Watch(this);

        InitializeComponent();
        SetPageService(pageService);
        navigationService.SetNavigationControl(RootNavigation);

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_chatClientService.Client.IsAdmin)
        {
            ViewModel.MenuItems = [
                new NavigationItem()
                {
                    Content = "管理",
                    Icon = SymbolRegular.Book24,
                    PageType = typeof(Views.Pages.ManagePage)
                }];
        }
        else
        {
            ViewModel.MenuItems = new()
            {
                new NavigationItem()
                {
                    Content = "消息",
                    Icon = SymbolRegular.Chat24,
                    PageType = typeof(Views.Pages.ChatPage)
                },
                new NavigationItem()
                {
                    Content = "联系人",
                    Icon = SymbolRegular.ContactCard24,
                    PageType = typeof(Views.Pages.ContactsPage)
                },
            };
        }
        ViewModel.MenuItems.Add(new NavigationItem()
        {
            Content = "Ai聊天",
            Icon = SymbolRegular.Bot24,
            PageType = typeof(Views.Pages.AiChatPage)
        });
    }

    #region INavigationWindow methods


    public INavigation GetNavigation() => RootNavigation;

    public Frame GetFrame() => RootFrame;
    public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);
    public void SetPageService(IPageService pageService) => RootNavigation.PageService = pageService;
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
            if (_chatClientService.Client.IsAdmin)
            {
                return;
            }

            ViewModel.Profile = await _chatClientService.Client.GetSelfAsync();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(this, $"无法加载配置文件. {ex.Message}", "错误");
        }
    }

    [RelayCommand]
    public void ShowSetProfileWindow()
    {
        using var scope = _serviceProvider.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<SetProfileWindow>();
        window.Owner = this;
        window.ShowDialog();

        _ = LoadProfileCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    public void MarkCurrentChatSessionMessageAsRead()
    {

    }
}

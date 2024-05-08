using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using StudentChat.Models;
using WpfStudentChat.Extensions;
using WpfStudentChat.Models.Messages;
using WpfStudentChat.Services;
using WpfStudentChat.ViewModels.Pages;
using WpfStudentChat.Views.Windows;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for GroupRequestsPage.xaml
/// </summary>
public partial class GroupRequestsPage : Page, 
    IRecipient<GroupRequestReceivedMessage>
{
    private readonly ChatClientService _chatClientService;
    public GroupRequestsViewModel ViewModel { get; }

    const int LoadCount = 20;

    public GroupRequestsPage(
        GroupRequestsViewModel viewModel, 
        ChatClientService chatClientService,
        IMessenger messenger)
    {
        _chatClientService = chatClientService;

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        messenger.Register(this);
    }


    [RelayCommand]
    public async Task LoadRequests()
    {
        try
        {
            var requests = await _chatClientService.Client.GetGroupRequestsAsync(0, LoadCount);
            ViewModel.Requests.Clear();
            foreach (var request in requests)
                ViewModel.Requests.Add(request);
        }
        catch (Exception ex)
        {
            MessageBox.Show(Application.Current.MainWindow, $"Failed to load requests. {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    public async Task LoadMoreRequests()
    {
        try
        {
            var requests = await _chatClientService.Client.GetGroupRequestsAsync(ViewModel.Requests.Count, LoadCount);
            foreach (var request in requests)
                ViewModel.Requests.Add(request);
        }
        catch (Exception ex)
        {
            MessageBox.Show(Application.Current.MainWindow, $"Failed to load more requests. {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    public async Task AcceptRequest(GroupRequest request)
    {
        try
        {
            await _chatClientService.Client.AcceptGroupRequestAsync(request.Id);
            var index = ViewModel.Requests.IndexOf(request);
            if (index != -1)
            {
                ViewModel.Requests[index] = request with { IsDone = true };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(App.Current.MainWindow, $"Failed to accept group request. {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    public async Task RejectRequest(GroupRequest request)
    {
        try
        {
            await _chatClientService.Client.RejectGroupRequestAsync(request.Id, null);
            var index = ViewModel.Requests.IndexOf(request);
            if (index != -1)
            {
                ViewModel.Requests[index] = request with { IsDone = true };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(App.Current.MainWindow, $"Failed to reject group request. {ex.Message}", "Error");
        }
    }


    void IRecipient<GroupRequestReceivedMessage>.Receive(GroupRequestReceivedMessage message)
    {
        ViewModel.Requests.Insert(0, message.Request);
    }
}

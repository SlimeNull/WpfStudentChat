using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using EleCho.WpfSuite;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using WpfStudentChat.Controls;
using WpfStudentChat.Services;

namespace WpfStudentChat.Views.Pages;

/// <summary>
/// Interaction logic for AiChatPage.xaml
/// </summary>
[ObservableObject]
public partial class AiChatPage : Page
{
    [ObservableProperty] private string _input = string.Empty;

    public ObservableCollection<Message> Messages { get; } = new();

    private readonly OpenAIClient _chat;
    private readonly ChatClientService _chatClientService;

    [ObservableProperty] private string _userName = string.Empty;

    private readonly SemaphoreSlim _chatLock = new(1);

    public AiChatPage(IConfiguration configuration, ChatClientService chatClientService)
    {
        DataContext = this;
        _chat = new OpenAIClient(new OpenAIAuthentication(configuration["OpenAi:Api-Key"]));
        InitializeComponent();
        _chatClientService = chatClientService; 

        if (_chatClientService.Client.IsAdmin)
        {
            UserName = "管理员";
        }
        else
        {
            _chatClientService.Client.GetUserAsync(_chatClientService.Client.GetSelfUserId())
            .ContinueWith(task =>
            {
                UserName = task.Result.Nickname;
            });
        }

    }

    private async void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                e.Handled = true;
                await Send();
                return;
            }
        }
    }

    [RelayCommand]
    public async Task Send()
    {
        await _chatLock.WaitAsync();
        try
        {
            const string ExceptionKey = "[Exception]";
            var newMessage = new Message(Role.User, Input);
            Messages.Add(newMessage);

            Scroll.ScrollToEnd();
            Input = string.Empty;
            try
            {
                var chatRequest = new ChatRequest(Messages.Append(newMessage).Where(v => v.Name != ExceptionKey), Model.GPT3_5_Turbo);
                var response = await _chat.ChatEndpoint.GetCompletionAsync(chatRequest);
                var choice = response.FirstChoice;

                Messages.Add(choice.Message);
            }
            catch (Exception ex)
            {
                Input = Messages[^1].Content;
                Messages.RemoveAt(Messages.Count - 1);
                TextBoxInput.SelectionStart = Input.Length;
                Messages.Add(new Message(Role.Assistant, $"错误\n{ex.Message}", ExceptionKey));
            }
            Scroll.ScrollToEnd();
        }
        finally
        {
            _chatLock.Release();
        }
    }
}

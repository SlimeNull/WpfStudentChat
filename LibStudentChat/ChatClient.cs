using LibStudentChat.Models.Events;

namespace LibStudentChat;

public class ChatClient
{
    private string? token;
    private CancellationTokenSource? backgroundTasksCancellation;

    private HttpClient _httpClient;

    public ChatClient(Uri baseUri)
    {
        ArgumentNullException.ThrowIfNull(baseUri, nameof(baseUri));

        _httpClient = new()
        {
            BaseAddress = baseUri,
        };
    }

    public Uri BaseUri => _httpClient.BaseAddress!;

    private async Task BackgroundTasks(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/Notify", UriKind.Relative));
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        
    }

    
    public async Task LoginAsync(string username, string password)
    {
        if (backgroundTasksCancellation is not null)
        {
            await backgroundTasksCancellation.CancelAsync();
        }


    }

    public event EventHandler<PrivateMessageReceivedEventArgs>? PrivateMessageReceived;
    public event EventHandler<GroupMessageReceivedEventArgs>? GroupMessageReceived;
    public event EventHandler<FriendRequestReceivedEventArgs>? FriendRequestReceived;
    public event EventHandler<GroupRequestReceivedEventArgs>? GroupRequestReceived;

    public event EventHandler<FriendChangedEventArgs>? FriendIncreased;
    public event EventHandler<FriendChangedEventArgs>? FriendDecreased;
    public event EventHandler<GroupChangedEventArgs>? GroupIncreased;
    public event EventHandler<GroupChangedEventArgs>? GroupDecreased;
}

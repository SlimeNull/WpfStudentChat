﻿using System.Diagnostics.Tracing;
using System.Text.Json;
using StudentChat.Models.Events;
using StudentChat.Utilites;

namespace StudentChat;

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
        const string EventNamePrivateMessage = "privateMessage";
        const string EventNameGroupMessage = "groupMessage";
        const string EventNameFriendRequest = "friendRequest";
        const string EventNameGroupRequest = "groupRequest";
        const string EventNameFriendIncreased = "friendIncreased";
        const string EventNameFriendDecreased = "friendDecreased";
        const string EventNameGroupIncreased = "groupIncreased";
        const string EventNameGroupDecreased = "groupDecreased";

        var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/Notify", UriKind.Relative));
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var serverSentEvents = ServerSentEvent.EnumerateFromStream(response.Content.ReadAsStream(), cancellationToken);
        
        await foreach (var serverSentEvent in serverSentEvents)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (serverSentEvent.Data is null)
            {
                continue;
            }

            try
            {
                switch (serverSentEvent.Event)
                {
                    case EventNamePrivateMessage:
                        PrivateMessageReceived?.Invoke(this, new PrivateMessageReceivedEventArgs(JsonSerializer.Deserialize<Models.PrivateMessage>(serverSentEvent.Data)!));
                        break;

                    case EventNameGroupMessage:
                        GroupMessageReceived?.Invoke(this, new GroupMessageReceivedEventArgs(JsonSerializer.Deserialize<Models.GroupMessage>(serverSentEvent.Data)!));
                        break;

                    case EventNameFriendRequest:
                        FriendRequestReceived?.Invoke(this, new FriendRequestReceivedEventArgs(JsonSerializer.Deserialize<Models.FriendRequest>(serverSentEvent.Data)!));
                        break;

                    case EventNameGroupRequest:
                        GroupRequestReceived?.Invoke(this, new GroupRequestReceivedEventArgs(JsonSerializer.Deserialize<Models.GroupRequest>(serverSentEvent.Data)!));
                        break;

                    case EventNameFriendIncreased:
                        FriendIncreased?.Invoke(this, new FriendChangedEventArgs(JsonSerializer.Deserialize<Models.User>(serverSentEvent.Data)!));
                        break;

                    case EventNameFriendDecreased:
                        FriendDecreased?.Invoke(this, new FriendChangedEventArgs(JsonSerializer.Deserialize<Models.User>(serverSentEvent.Data)!));
                        break;

                    case EventNameGroupIncreased:
                        GroupIncreased?.Invoke(this, new GroupChangedEventArgs(JsonSerializer.Deserialize<Models.Group>(serverSentEvent.Data)!));
                        break;

                    case EventNameGroupDecreased:
                        GroupDecreased?.Invoke(this, new GroupChangedEventArgs(JsonSerializer.Deserialize<Models.Group>(serverSentEvent.Data)!));
                        break;
                }
            }
            catch
            {
                // TODO: log here
            }
        }
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

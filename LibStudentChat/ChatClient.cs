﻿using System.Diagnostics.Tracing;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StudentChat.Models;
using StudentChat.Models.Events;
using StudentChat.Models.Network;
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
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var serverSentEvents = ServerSentEvent.EnumerateFromStream(response.Content.ReadAsStream(), cancellationToken);

#if DEBUG
        await Console.Out.WriteLineAsync("Notify events ok");
#endif

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

    private async Task<TResultData> PostAsync<TRequestData, TResultData>(string path, TRequestData requestData)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Content = JsonContent.Create(requestData);

        if (token is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);

        var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<TResultData>>();

        if (apiResult is null)
        {
            throw new Exception("Server returns empty result");
        }

        if (!apiResult.Ok || apiResult.Data is null)
        {
            throw new Exception(apiResult.Message);
        }

        return apiResult.Data;
    }

    private async Task PostAsync<TRequestData>(string path, TRequestData requestData)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Content = JsonContent.Create(requestData);

        if (token is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);
        var apiResult = await response.Content.ReadFromJsonAsync<ApiResult>();

        if (apiResult is null)
        {
            throw new Exception("Server returns empty result");
        }

        if (!apiResult.Ok)
        {
            throw new Exception(apiResult.Message);
        }
    }

    private async Task<BinaryUploadResultData> UploadBinary(string path, string parameterName, byte[] fileContent, int offset, int count, string contentType)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);
        var requestContent = new MultipartFormDataContent();
        request.Content = requestContent;

        requestContent.Add(new ByteArrayContent(fileContent, offset, count), parameterName);

        if (token is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);
        var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<BinaryUploadResultData>>();

        if (apiResult is null)
        {
            throw new Exception("Server returns empty result");
        }

        if (!apiResult.Ok || apiResult.Data is null)
        {
            throw new Exception(apiResult.Message);
        }

        return apiResult.Data;
    }

    public async Task LoginAsync(string username, string password)
    {
        if (backgroundTasksCancellation is not null)
        {
            await backgroundTasksCancellation.CancelAsync();
        }

        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] passwordHash = SHA256.HashData(passwordBytes);
        string passwordHashText = Convert.ToHexString(passwordHash);

        var result = await PostAsync<LoginRequestData, LoginResultData>(
            "/api/Auth/Login", 
            new LoginRequestData(username, passwordHashText));

        token = result.Token;
        backgroundTasksCancellation = new();

        // start background task
        _ = BackgroundTasks(backgroundTasksCancellation.Token);
    }

    public async Task<List<PrivateMessage>> QueryPrivateMessages(int userId, DateTimeOffset? startTime, DateTimeOffset? endTime, int count)
    {
        var result = await PostAsync<QueryPrivateMessagesRequestData, QueryPrivateMessagesResultData>(
            "api/Chat/QueryPrivateMessages", 
            new QueryPrivateMessagesRequestData(userId, startTime, endTime, count));

        return result.Messages;
    }

    public async Task<List<GroupMessage>> QueryGroupMessages(int groupId, DateTimeOffset? startTime, DateTimeOffset? endTime, int count)
    {
        var result = await PostAsync<QueryGroupMessagesRequestData, QueryGroupMessagesResultData>(
            "api/Chat/QueryGroupMessages",
            new QueryGroupMessagesRequestData(groupId, startTime, endTime, count));

        return result.Messages;
    }

    public async Task SendPrivateMessage(int receiverId, string content, List<Attachment>? imageAttachments, List<Attachment>? fileAttachments)
    {
        await PostAsync<SendPrivateMessageRequestData>(
            "api/Chat/SendPrivateMessage",
            new SendPrivateMessageRequestData(receiverId, content, imageAttachments, fileAttachments));
    }

    public async Task SendGroupMessage(int groupId, string content, List<Attachment>? imageAttachments, List<Attachment>? fileAttachments)
    {
        await PostAsync<SendGroupMessageRequestData>(
            "api/Chat/SendGroupMessage",
            new SendGroupMessageRequestData(groupId, content, imageAttachments, fileAttachments));
    }

    public async Task SendFriendRequest(int userId, string? message)
    {
        await PostAsync<SendFriendRequestRequestData>(
            "api/Request/SendFriendRequest",
            new SendFriendRequestRequestData(userId, message));
    }

    public async Task SendGroupRequest(int groupId, string? message)
    {
        await PostAsync<SendGroupRequestRequestData>(
            "api/Request/SendGroupRequest",
            new SendGroupRequestRequestData(groupId, message));
    }

    public async Task AcceptFriendRequest(int requestId)
    {
        await PostAsync<AcceptRequestRequestData>(
            "api/Request/AcceptFriendRequest",
            new AcceptRequestRequestData(requestId));
    }

    public async Task AcceptGroupRequest(int requestId)
    {
        await PostAsync<AcceptRequestRequestData>(
            "api/Request/AcceptGroupRequest",
            new AcceptRequestRequestData(requestId));
    }

    public async Task RejectFriendRequest(int requestId, string? reason)
    {
        await PostAsync<RejectRequestRequestData>(
            "api/Request/RejectFriendRequest",
            new RejectRequestRequestData(requestId, reason));
    }

    public async Task RejectGroupRequest(int requestId, string? reason)
    {
        await PostAsync<RejectRequestRequestData>(
            "api/Request/RejectGroupRequest",
            new RejectRequestRequestData(requestId, reason));
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

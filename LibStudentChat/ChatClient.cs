using System.Diagnostics.Tracing;
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

    private readonly HttpClient _httpClient;

    public ChatClient(Uri baseUri)
    {
        ArgumentNullException.ThrowIfNull(baseUri, nameof(baseUri));

        _httpClient = new()
        {
            BaseAddress = baseUri,
        };
    }

    public Uri BaseUri => _httpClient.BaseAddress!;

    public int GetSelfUserId()
    {
        const string UserIdKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        if (token is null)
            return -1;

        var index1 = token.IndexOf('.');
        if (index1 == -1)
            return -1;
        var index2 = token.IndexOf('.', index1 + 1);
        if (index2 == -1)
            return -1;
        
        var payload = token[(index1 + 1)..index2];
        var padLength = 4 - payload.Length % 4;
        if (padLength == 4)
            padLength = 0;
        var payloadBytes = Convert.FromBase64String(payload.PadRight(payload.Length + padLength, '='));
        var payloadText = Encoding.UTF8.GetString(payloadBytes);
        var payloadJson = JsonSerializer.Deserialize<JsonElement>(payloadText);
        if (!payloadJson.TryGetProperty(UserIdKey, out var userId))
            return -1;

        return Convert.ToInt32(userId.GetString());
    }

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

    private async Task<TResultData> PostAsync<TResultData>(string path)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);;

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

    private async Task<BinaryUploadResultData> UploadBinary(string path, string parameterName, string fileName, byte[] fileContent, int offset, int count, string contentType)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);
        var requestContent = new MultipartFormDataContent();
        request.Content = requestContent;

        requestContent.Add(new ByteArrayContent(fileContent, offset, count), parameterName, fileName);

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

    private async Task<Stream?> DownloadBinary(string path)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (token is not null)
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            return await response.Content.ReadAsStreamAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var result = await UploadBinary("/api/Binary/UploadImage", "file", fileName, ms.ToArray(), 0, (int)ms.Length, "image/jpeg");

        return result.Hash;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var result = await UploadBinary("/api/Binary/UploadFile", "file", fileName, ms.ToArray(), 0, (int)ms.Length, "image/jpeg");

        return result.Hash;
    }

    public Task<Stream?> GetImageAsync(string hash)
        => DownloadBinary($"api/Binary/GetImage/{hash}");

    public Task<Stream?> GetFileAsync(string hash)
        => DownloadBinary($"api/Binary/GetFile/{hash}");

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

    public async Task<List<PrivateMessage>> QueryPrivateMessagesAsync(int userId, DateTimeOffset? startTime, DateTimeOffset? endTime, int count)
    {
        var result = await PostAsync<QueryPrivateMessagesRequestData, QueryPrivateMessagesResultData>(
            "api/Chat/QueryPrivateMessages", 
            new QueryPrivateMessagesRequestData(userId, startTime, endTime, count));

        return result.Messages;
    }

    public async Task<List<GroupMessage>> QueryGroupMessagesAsync(int groupId, DateTimeOffset? startTime, DateTimeOffset? endTime, int count)
    {
        var result = await PostAsync<QueryGroupMessagesRequestData, QueryGroupMessagesResultData>(
            "api/Chat/QueryGroupMessages",
            new QueryGroupMessagesRequestData(groupId, startTime, endTime, count));

        return result.Messages;
    }

    public async Task SendPrivateMessageAsync(int receiverId, string content, List<Attachment>? imageAttachments, List<Attachment>? fileAttachments)
    {
        await PostAsync<SendPrivateMessageRequestData>(
            "api/Chat/SendPrivateMessage",
            new SendPrivateMessageRequestData(receiverId, content, imageAttachments, fileAttachments));
    }

    public async Task SendGroupMessageAsync(int groupId, string content, List<Attachment>? imageAttachments, List<Attachment>? fileAttachments)
    {
        await PostAsync<SendGroupMessageRequestData>(
            "api/Chat/SendGroupMessage",
            new SendGroupMessageRequestData(groupId, content, imageAttachments, fileAttachments));
    }

    public async Task SendFriendRequestAsync(int userId, string? message)
    {
        await PostAsync<SendFriendRequestRequestData>(
            "api/Request/SendFriendRequest",
            new SendFriendRequestRequestData(userId, message));
    }

    public async Task SendGroupRequestAsync(int groupId, string? message)
    {
        await PostAsync<SendGroupRequestRequestData>(
            "api/Request/SendGroupRequest",
            new SendGroupRequestRequestData(groupId, message));
    }

    public async Task AcceptFriendRequestAsync(int requestId)
    {
        await PostAsync<AcceptRequestRequestData>(
            "api/Request/AcceptFriendRequest",
            new AcceptRequestRequestData(requestId));
    }

    public async Task AcceptGroupRequestAsync(int requestId)
    {
        await PostAsync<AcceptRequestRequestData>(
            "api/Request/AcceptGroupRequest",
            new AcceptRequestRequestData(requestId));
    }

    public async Task RejectFriendRequestAsync(int requestId, string? reason)
    {
        await PostAsync<RejectRequestRequestData>(
            "api/Request/RejectFriendRequest",
            new RejectRequestRequestData(requestId, reason));
    }

    public async Task RejectGroupRequestAsync(int requestId, string? reason)
    {
        await PostAsync<RejectRequestRequestData>(
            "api/Request/RejectGroupRequest",
            new RejectRequestRequestData(requestId, reason));
    }

    public async Task<User> GetSelfAsync()
    {
        var result = await PostAsync<GetUserResultData>(
            "api/Info/GetSelf");

        return result.User;
    }

    public async Task SetSelfAsync(User profile)
    {
        await PostAsync<SetUserRequestData>(
            "api/Info/SetSelf",
            new SetUserRequestData(profile));
    }

    public async Task SetSelfPasswordAsync(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] passwordHash = SHA256.HashData(passwordBytes);
        string passwordHashText = Convert.ToHexString(passwordHash);

        await PostAsync<SetPasswordRequestData>(
            "api/Info/SetSelfPassword",
            new SetPasswordRequestData(passwordHashText));
    }

    public async Task<User> GetUserAsync(int userId)
    {
        var result = await PostAsync<GetUserRequestData, GetUserResultData>(
            "api/Info/GetUser",
            new GetUserRequestData(userId));

        return result.User;
    }

    public async Task<Group> GetGroupAsync(int groupId)
    {
        var result = await PostAsync<GetGroupRequestData, GetGroupResultData>(
            "api/Info/GetGroup",
            new GetGroupRequestData(groupId));

        return result.Group;
    }

    public async Task SetGroupAsync(Group group)
    {
        await PostAsync<SetGroupRequestData>(
            "api/Info/SetGroup",
            new SetGroupRequestData(group));
    }

    public async Task<Group> CreateGroupAsync(Group group)
    {
        var result = await PostAsync<CreateGroupRequestData, CreateGroupResultData>(
            "api/Info/CreateGroup",
            new CreateGroupRequestData(group));

        return result.Group;
    }

    public async Task DeleteGroupAsync(int groupId)
    {
        await PostAsync<DeleteGroupRequestData>(
            "api/Info/DeleteGroup",
            new DeleteGroupRequestData(groupId));
    }

    public async Task<List<User>> SearchUserAsync(string keyword, int skip, int count = 30)
    {
        var result = await PostAsync<SearchUserRequestData, SearchUserResultData>(
            "api/Info/SearchUser",
            new SearchUserRequestData(keyword, skip, count));

        return result.Users;
    }

    public async Task<List<Group>> SearchGroupAsync(string keyword, int skip, int count = 30)
    {
        var result = await PostAsync<SearchGroupRequestData, SearchGroupResultData>(
            "api/Info/SearchGroup",
            new SearchGroupRequestData(keyword, skip, count));

        return result.Groups;
    }

    public async Task<List<User>> GetFriendsAsync()
    {
        var result = await PostAsync<GetFriendsResultData>(
            "api/Info/GetFriends");

        return result.Friends;
    }

    public async Task<List<Group>> GetGroupsAsync()
    {
        var result = await PostAsync<GetGroupsResultData>(
            "api/Info/GetGroups");

        return result.Groups;
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

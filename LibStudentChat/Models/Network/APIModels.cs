using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentChat.Models;

namespace StudentChat.Models.Network
{
    public record LoginRequestData(string UserName, string PasswordHash);
    public record LoginResultData(string Token);



    public record BinaryUploadResultData(string Hash);


    public record class QueryPrivateMessagesRequestData(int UserId, DateTimeOffset? StartTime, DateTimeOffset? EndTime, int Count);
    public record class QueryPrivateMessagesResultData(PrivateMessage[] Messages);

    public record class QueryGroupMessagesRequestData(int GroupId, DateTimeOffset? StartTime, DateTimeOffset? EndTime, int Count);
    public record class QueryGroupMessagesResultData(GroupMessage[] Messages);

    public record class SendPrivateMessageRequestData(int ReceiverId, string Content, Attachment[]? ImageAttachments, Attachment[]? FileAttachments);
    public record class SendGroupMessageRequestData(int GroupId, string Content, Attachment[]? ImageAttachments, Attachment[]? FileAttachments);



    public record class AddUserRequestData(User User, string PasswordHash);
    public record class AddUserResultData(User User);

    public record class GetUsersRequestData(string UserNameKeyword, string NicknameKeyword, int Skip, int Count);
    public record class GetUsersResultData(int TotalCount, User[] Users);


    public record class GetUserRequestData(int UserId);
    public record class GetUserResultData(User User);
    public record class GetGroupRequestData(int GroupId);
    public record class GetGroupResultData(Group Group);
    public record class SetUserRequestData(User User);
    public record class SetGroupRequestData(Group Group);
    public record class SetPasswordRequestData(string PasswordHash);
    public record class CreateGroupRequestData(Group Group);
    public record class CreateGroupResultData(Group Group);
    public record class DeleteGroupRequestData(int GroupId);


    public record class SearchUserResultData(User[] Users);
    public record class SearchGroupResultData(Group[] Groups);


    public record class GetFriendsResultData(User[] Friends);
    public record class GetGroupsResultData(Group[] Groups);


    public record class GetSentFriendRequestsResultData(FriendRequest[] Requests);
    public record class GetReceivedFriendRequestsResultData(FriendRequest[] Requests);
    public record class GetSentGroupRequestsResultData(GroupRequest[] Requests);
    public record class GetReceivedGroupRequestsResultData(GroupRequest[] Requests);
    public record class GetFriendRequestsResultData(FriendRequest[] Requests);
    public record class GetGroupRequestsResultData(GroupRequest[] Requests);

    /// <summary>
    /// 发送好友请求的请求数据
    /// </summary>
    /// <param name="UserId">要添加的好友的 ID</param>
    /// <param name="Message">附加消息</param>
    public record class SendFriendRequestRequestData(int UserId, string? Message);
    public record class SendGroupRequestRequestData(int GroupId, string? Message);
    public record class AcceptRequestRequestData(int RequestId);
    public record class RejectRequestRequestData(int RequestId, string? Reason);


    public record class QueryRequestData(int Skip, int Count);
    public record class KeywordQueryRequestData(string Keyword, int Skip, int Count);
}

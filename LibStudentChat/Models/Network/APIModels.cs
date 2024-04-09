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
    public record class QueryPrivateMessagesResultData(List<PrivateMessage> Messages);

    public record class QueryGroupMessagesRequestData(int GroupId, DateTimeOffset? StartTime, DateTimeOffset? EndTime, int Count);
    public record class QueryGroupMessagesResultData(List<GroupMessage> Messages);

    public record class SendPrivateMessageRequestData(int ReceiverId, string Content, List<Attachment>? ImageAttachments, List<Attachment>? FileAttachments);
    public record class SendGroupMessageRequestData(int GroupId, string Content, List<Attachment>? ImageAttachments, List<Attachment>? FileAttachments);



    public record class AddUserRequestData(User User, string PasswordHash);
    public record class AddUserResultData(User User);


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


    public record class SearchUserResultData(List<User> Users);
    public record class SearchGroupResultData(List<Group> Groups);


    public record class GetFriendsResultData(List<User> Friends);
    public record class GetGroupsResultData(List<Group> Groups);


    public record class GetSentFriendRequestsResultData(List<FriendRequest> Requests);
    public record class GetReceivedFriendRequestsResultData(List<FriendRequest> Requests);
    public record class GetSentGroupRequestsResultData(List<GroupRequest> Requests);
    public record class GetReceivedGroupRequestsResultData(List<GroupRequest> Requests);
    public record class GetFriendRequestsResultData(List<FriendRequest> Requests);
    public record class GetGroupRequestsResultData(List<GroupRequest> Requests);

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

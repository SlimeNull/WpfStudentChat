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

    public record class SendPrivateMessageRequestData(PrivateMessage Message);
    public record class SendGroupMessageRequestData(GroupMessage Message);



    public record class AddUserRequestData(User User, string PasswordHash);
    public record class AddUserResultData(User User);


    /// <summary>
    /// 发送好友请求的请求数据
    /// </summary>
    /// <param name="UserId">要添加的好友的 ID</param>
    /// <param name="Message">附加消息</param>
    public record class SendFriendRequestRequestData(int UserId, string Message);
    public record class SendGroupRequestRequestData(int GroupId, string Message);
    public record class DealRequestRequestData(int RequestId);
}

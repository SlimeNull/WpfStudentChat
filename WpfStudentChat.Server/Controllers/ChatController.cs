using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WpfStudentChat.Server.Extensions;
using WpfStudentChat.Server.Models;
using WpfStudentChat.Server.Models.Database;
using WpfStudentChat.Server.Services;
using CommonModels = LibStudentChat.Models;

namespace WpfStudentChat.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;
        private readonly NotifyService _messageNotifyService;

        public record class QueryPrivateMessagesRequestData(int UserId, DateTimeOffset? StartTime, DateTimeOffset? EndTime, int Count);
        public record class QueryPrivateMessagesResultData(List<CommonModels.PrivateMessage> Messages);

        public record class QueryGroupMessagesRequestData(int GroupId, DateTimeOffset? StartTime, DateTimeOffset? EndTime, int Count);
        public record class QueryGroupMessagesResultData(List<CommonModels.GroupMessage> Messages);

        public record class SendPrivateMessageRequestData(CommonModels.PrivateMessage Message);
        public record class SendGroupMessageRequestData(CommonModels.GroupMessage Message);



        public ChatController(
            ChatServerDbContext dbContext,
            NotifyService messageNotifyService)
        {
            _dbContext = dbContext;
            _messageNotifyService = messageNotifyService;
        }


        [HttpPost("QueryPrivateMessages")]
        public ApiResult<QueryPrivateMessagesResultData> QueryPrivateMessages(QueryPrivateMessagesRequestData request)
        {
            throw new NotImplementedException();
        }


        [HttpPost("QueryGroupMessages")]
        public ApiResult<QueryGroupMessagesResultData> QueryGroupMessages(QueryGroupMessagesRequestData request)
        {
            throw new NotImplementedException();
        }


        [HttpPost("SendPrivateMessage")]
        public async Task<ApiResult> SendPrivateMessageAsync(SendPrivateMessageRequestData request)
        {
            int selfUserId = HttpContext.GetUserId();
            bool selfHasFriend = await _dbContext.CheckUserHasFriendAsync(selfUserId, request.Message.ReceiverId);

            if (!selfHasFriend)
            {
                return ApiResult.CreateErr("No such friend");
            }

            var entry = _dbContext.PrivateMessages.Add(
                new PrivateMessage()
                {
                    SenderId = selfUserId,
                    ReceiverId = request.Message.ReceiverId,
                    Content = request.Message.Content,
                    SentTime = DateTimeOffset.Now,
                });

            await _dbContext.SaveChangesAsync();
            await _messageNotifyService.OnPrivateMessageSent((CommonModels.PrivateMessage)entry.Entity);

            return ApiResult.CreateOk();
        }


        [HttpPost("SendGroupMessage")]
        public async Task<ApiResult> SendGroupMessageAsync(SendGroupMessageRequestData request)
        {
            int selfUserId = HttpContext.GetUserId();
            bool selfHasGroup = await _dbContext.CheckUserHasGroupAsync(selfUserId, request.Message.GroupId);

            if (!selfHasGroup)
            {
                return ApiResult.CreateErr("No such group");
            }

            var entry = _dbContext.GroupMessages.Add(
                new GroupMessage()
                {
                    SenderId = selfUserId,
                    GroupId = request.Message.GroupId,
                    Content = request.Message.Content,
                    SentTime = DateTimeOffset.Now,
                });

            await _dbContext.SaveChangesAsync();
            await _messageNotifyService.OnGroupMessageSent((CommonModels.GroupMessage)entry.Entity);

            return ApiResult.CreateOk();
        }
    }
}
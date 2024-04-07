using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WpfStudentChat.Server.Models;
using WpfStudentChat.Server.Models.Database;
using WpfStudentChat.Server.Services;
using CommonModels = LibStudentChat.Models;

namespace WpfStudentChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;
        private readonly MessageNotifyService _messageNotifyService;

        public record class QueryPrivateMessagesRequestData(int UserId, DateTimeOffset StartTime, DateTimeOffset EndTime, int Count);
        public record class QueryPrivateMessagesResultData(List<CommonModels.PrivateMessage> Messages);

        public record class QueryGroupMessagesRequestData(int GroupId, DateTimeOffset StartTime, DateTimeOffset EndTime, int Count);
        public record class QueryGroupMessagesResultData(List<CommonModels.GroupMessage> Messages);


        public ChatController(
            ChatServerDbContext dbContext,
            MessageNotifyService messageNotifyService)
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

        [HttpGet("StreamPrivateMessages")]
        public async Task StreamPrivateMessages()
        {
            HttpContext.Response.ContentType = "text/event-stream";

            try
            {
                _messageNotifyService.PrivateMessageSent += MessageNotifyService_PrivateMessageSent;
                await Task.Delay(-1);
            }
            finally
            {
                _messageNotifyService.PrivateMessageSent -= MessageNotifyService_PrivateMessageSent;
            }
        }

        [HttpGet("StreamGroupMessages")]
        public async Task StreamGroupMessages()
        {
            HttpContext.Response.ContentType = "text/event-stream";

            try
            {
                _messageNotifyService.GroupMessageSent += MessageNotifyService_GroupMessageSent;
                await Task.Delay(-1);
            }
            finally
            {
                _messageNotifyService.GroupMessageSent -= MessageNotifyService_GroupMessageSent;
            }
        }

        private void MessageNotifyService_GroupMessageSent(object? sender, MessageNotifyService.GroupMessageSentEventArgs e)
        {
            
        }

        private void MessageNotifyService_PrivateMessageSent(object? sender, MessageNotifyService.PrivateMessageSentEventArgs e)
        {

        }
    }
}
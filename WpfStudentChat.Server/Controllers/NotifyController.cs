using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WpfStudentChat.Server.Extensions;
using WpfStudentChat.Server.Services;
using WpfStudentChat.Server.Models.Database;

namespace WpfStudentChat.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;
        private readonly NotifyService _notifyService;

        public NotifyController(
            ChatServerDbContext dbContext,
            NotifyService notifyService)
        {
            _dbContext = dbContext;
            _notifyService = notifyService;
        }


        [HttpGet]
        public async Task Get()
        {
            HttpContext.Response.ContentType = "text/event-stream";

            try
            {
                _notifyService.PrivateMessageSent += MessageNotifyService_PrivateMessageSent;
                _notifyService.GroupMessageSent += MessageNotifyService_GroupMessageSent;
                await Task.Delay(-1);
            }
            finally
            {
                _notifyService.PrivateMessageSent -= MessageNotifyService_PrivateMessageSent;
                _notifyService.GroupMessageSent -= MessageNotifyService_GroupMessageSent;
            }
        }


        private async Task MessageNotifyService_PrivateMessageSent(object? sender, NotifyService.PrivateMessageSentEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();

            if (e.Message.SenderId != selfUserId && e.Message.ReceiverId != selfUserId)
                return;

            string text =
                $"""
                event: privateMessage
                data: {JsonSerializer.Serialize(e.Message)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_GroupMessageSent(object? sender, NotifyService.GroupMessageSentEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var selfJoinedGroup = _dbContext.GroupMembers.Any(gm => gm.UserId == selfUserId && e.Message.GroupId == e.Message.GroupId);

            if (!selfJoinedGroup)
                return;

            string text =
                $"""
                event: groupMessage
                data: {JsonSerializer.Serialize(e.Message)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }
    }
}

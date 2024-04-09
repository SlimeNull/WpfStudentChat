using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Server.Extensions;
using StudentChat.Server.Services;
using StudentChat.Server.Models.Database;

namespace StudentChat.Server.Controllers
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
                _notifyService.FriendRequestSent += MessageNotifyService_FriendRequestSent;
                _notifyService.GroupRequestSent += MessageNotifyService_GroupRequestSent;
                _notifyService.FriendIncreased += MessageNotifyService_FriendIncreased;
                _notifyService.FriendDecreased += MessageNotifyService_FriendDecreased;
                _notifyService.GroupIncreased += MessageNotifyService_GroupIncreased;
                _notifyService.GroupDecreased += MessageNotifyService_GroupDecreased;

                await Task.Delay(-1);
            }
            finally
            {
                _notifyService.PrivateMessageSent -= MessageNotifyService_PrivateMessageSent;
                _notifyService.GroupMessageSent -= MessageNotifyService_GroupMessageSent;
                _notifyService.FriendRequestSent -= MessageNotifyService_FriendRequestSent;
                _notifyService.GroupRequestSent -= MessageNotifyService_GroupRequestSent;
                _notifyService.FriendIncreased -= MessageNotifyService_FriendIncreased;
                _notifyService.FriendDecreased -= MessageNotifyService_FriendDecreased;
                _notifyService.GroupIncreased -= MessageNotifyService_GroupIncreased;
                _notifyService.GroupDecreased -= MessageNotifyService_GroupDecreased;
            }
        }


        private async Task MessageNotifyService_PrivateMessageSent(object? sender, NotifyService.PrivateMessageSentEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated = e.Message.SenderId == selfUserId || e.Message.ReceiverId == selfUserId;

            if (!isRelated)
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
            var isRelated = await _dbContext.CheckUserHasGroupAsync(selfUserId, e.Message.GroupId);

            if (!isRelated)
                return;

            string text =
                $"""
                event: groupMessage
                data: {JsonSerializer.Serialize(e.Message)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_FriendRequestSent(object? sender, NotifyService.FriendRequestSentEvnetArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated = _dbContext.FriendRequests.Any(r => r.SenderId == selfUserId || r.ReceiverId == selfUserId);

            if (!isRelated)
                return;

            string text =
                $"""
                event: friendRequest
                data: {JsonSerializer.Serialize(e.Request)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_GroupRequestSent(object? sender, NotifyService.GroupRequestSentEvnetArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated1 = _dbContext.GroupRequests.Any(r => r.SenderId == selfUserId);
            var isRelated2 = _dbContext.Users
                .Where(user => user.Id == selfUserId)
                .Include(entity => entity.OwnedGroups)
                .SelectMany(user => user.OwnedGroups)
                .Any(group => group.Id == e.Request.GroupId);

            if (!isRelated1 && !isRelated2)
                return;

            string text =
                $"""
                event: groupRequest
                data: {JsonSerializer.Serialize(e.Request)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_FriendIncreased(object? sender, NotifyService.FriendChangedEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated = selfUserId== e.UserId;

            if (!isRelated)
            {
                return;
            }

            string text =
                $"""
                event: friendIncreased
                data: {JsonSerializer.Serialize(e.Friend)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_FriendDecreased(object? sender, NotifyService.FriendChangedEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated = selfUserId== e.UserId;

            if (!isRelated)
            {
                return;
            }

            string text =
                $"""
                event: friendDecreased
                data: {JsonSerializer.Serialize(e.Friend)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_GroupIncreased(object? sender, NotifyService.GroupChangedEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated = selfUserId== e.UserId;

            if (!isRelated)
            {
                return;
            }

            string text =
                $"""
                event: groupIncreased
                data: {JsonSerializer.Serialize(e.Group)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }

        private async Task MessageNotifyService_GroupDecreased(object? sender, NotifyService.GroupChangedEventArgs e)
        {
            var selfUserId = HttpContext.GetUserId();
            var isRelated = selfUserId== e.UserId;

            if (!isRelated)
            {
                return;
            }

            string text =
                $"""
                event: groupDecreased
                data: {JsonSerializer.Serialize(e.Group)}


                """;

            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
            await HttpContext.Response.Body.FlushAsync();
        }
    }
}

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
    public class RequestController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;
        private readonly NotifyService _notifyService;

        /// <summary>
        /// 发送好友请求的请求数据
        /// </summary>
        /// <param name="UserId">要添加的好友的 ID</param>
        /// <param name="Message">附加消息</param>
        public record class SendFriendRequestRequestData(int UserId, string Message);
        public record class SendGroupRequestRequestData(int GroupId, string Message);
        public record class DealRequestRequestData(int RequestId);

        public RequestController(
            ChatServerDbContext dbContext,
            NotifyService notifyService)
        {
            _dbContext = dbContext;
            _notifyService = notifyService;
        }

        [HttpPost("SendFriendRequest")]
        public async Task<ApiResult> SendFriendRequestAsync(SendFriendRequestRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var alreadyExist = _dbContext.FriendRequests.Any(r => r.SenderId == selfUserId && r.ReceiverId == request.UserId);

            if (alreadyExist)
            {
                return ApiResult.CreateErr("已经发送过请求了");
            }

            var entry = _dbContext.FriendRequests.Add(
                new FriendRequest()
                {
                    SenderId = selfUserId,
                    ReceiverId = request.UserId,
                    Message = request.Message,
                });

            await _dbContext.SaveChangesAsync();
            await _notifyService.OnFriendRequestSent((CommonModels.FriendRequest)entry.Entity);

            return ApiResult.CreateOk();
        }

        [HttpPost("SendGroupRequest")]
        public async Task<ApiResult> SendGroupRequestAsync(SendGroupRequestRequestData request)
        {
            var selfUserId = HttpContext.GetUserId();
            var alreadyExist = _dbContext.GroupRequests.Any(r => r.SenderId == selfUserId && r.GroupId == request.GroupId);

            if (alreadyExist)
            {
                return ApiResult.CreateErr("已经发送过请求了");
            }

            var entry = _dbContext.GroupRequests.Add(
                new GroupRequest()
                {
                    SenderId = selfUserId,
                    GroupId = request.GroupId,
                    Message = request.Message,
                });

            await _dbContext.SaveChangesAsync();
            await _notifyService.OnGroupRequestSent((CommonModels.GroupRequest)entry.Entity);

            return ApiResult.CreateOk();
        }


        [HttpPost("AcceptFriendRequest")]
        public Task<ApiResult> AcceptFriendRequestAsync(DealRequestRequestData request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("RejectFriendRequest")]
        public Task<ApiResult> RejectFriendRequestAsync(DealRequestRequestData request)
        {
            throw new NotImplementedException();

        }

        [HttpPost("AcceptGroupRequest")]
        public Task<ApiResult> AcceptGroupRequestAsync(DealRequestRequestData request)
        {
            throw new NotImplementedException();

        }

        [HttpPost("RejectGroupRequest")]
        public Task<ApiResult> RejectGroupRequestAsync(DealRequestRequestData request)
        {
            throw new NotImplementedException();

        }
    }
}

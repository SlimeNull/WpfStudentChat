using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentChat.Models.Network;
using StudentChat.Server.Extensions;
using StudentChat.Server.Models;
using StudentChat.Server.Models.Database;
using StudentChat.Server.Services;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;
        private readonly NotifyService _notifyService;

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

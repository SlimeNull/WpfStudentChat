using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return ApiResult.CreateErr("You have already sent a request");
            }

            var userExist = await _dbContext.Users
                .AnyAsync(user => user.Id == request.UserId);

            if (!userExist)
            {
                return ApiResult.CreateErr("No such user");
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
                return ApiResult.CreateErr("You have already sent a request");
            }

            var groupExist = await _dbContext.Groups
                .AnyAsync(group => group.Id == request.GroupId);

            if (!groupExist)
            {
                return ApiResult.CreateErr("No such group");
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
        public async Task<ApiResult> AcceptFriendRequestAsync(AcceptRequestRequestData request)
        {
            var selfId = HttpContext.GetUserId();
            var friendRequest = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(fr => fr.ReceiverId == selfId && fr.Id == request.RequestId);

            if (friendRequest is null)
            {
                return ApiResult.CreateErr("No such friend request");
            }

            if (friendRequest.IsDone)
            {
                return ApiResult.CreateErr("This request has already been processed");
            }

            friendRequest.IsDone = true;

            await _dbContext.UserFriends.AddAsync(
                new UserFriend()
                {
                    FromUserId = friendRequest.SenderId,
                    ToUserId = selfId,
                });

            await _dbContext.SaveChangesAsync();

            var requestSender = _dbContext.Users.First(u => u.Id == friendRequest.SenderId);
            var requestReceiver = _dbContext.Users.First(u => u.Id == friendRequest.ReceiverId);
            await _notifyService.OnFriendIncreased(requestSender.Id, (CommonModels.User)requestReceiver);
            await _notifyService.OnFriendIncreased(requestReceiver.Id, (CommonModels.User)requestSender);

            return ApiResult.CreateOk();
        }

        [HttpPost("RejectFriendRequest")]
        public async Task<ApiResult> RejectFriendRequestAsync(AcceptRequestRequestData request)
        {
            var selfId = HttpContext.GetUserId();
            var friendRequest = await _dbContext.FriendRequests
                .FirstOrDefaultAsync(fr => fr.ReceiverId == selfId && fr.Id == request.RequestId);

            if (friendRequest is null)
            {
                return ApiResult.CreateErr("No such friend request");
            }

            if (friendRequest.IsDone)
            {
                return ApiResult.CreateErr("This request has already been processed");
            }

            friendRequest.IsDone = true;

            //await _dbContext.UserFriends.AddAsync(
            //    new UserFriend()
            //    {
            //        FromUserId = friendRequest.SenderId,
            //        ToUserId = selfId,
            //    });

            await _dbContext.SaveChangesAsync();

            //var requestSender = _dbContext.Users.First(u => u.Id == friendRequest.SenderId);
            //var requestReceiver = _dbContext.Users.First(u => u.Id == friendRequest.ReceiverId);
            //await _notifyService.OnFriendIncreased(requestSender.Id, (CommonModels.User)requestReceiver);
            //await _notifyService.OnFriendIncreased(requestReceiver.Id, (CommonModels.User)requestSender);

            return ApiResult.CreateOk();
        }

        [HttpPost("AcceptGroupRequest")]
        public async Task<ApiResult> AcceptGroupRequestAsync(AcceptRequestRequestData request)
        {
            var selfId = HttpContext.GetUserId();
            var groupRequest = await _dbContext.GroupRequests
                .FirstOrDefaultAsync(gr => gr.Id == request.RequestId);

            if (groupRequest is null)
            {
                return ApiResult.CreateErr("No such group request");
            }

            if (groupRequest.IsDone)
            {
                return ApiResult.CreateErr("This request has already been processed");
            }

            var group = await _dbContext.Groups
                .AsNoTracking()
                .FirstAsync(g => g.Id == groupRequest.GroupId);

            if (group.OwnerId != selfId)
            {
                return ApiResult.CreateErr("You are not owner of that group");
            }

            groupRequest.IsDone = true;

            await _dbContext.GroupMembers.AddAsync(
                new GroupMember()
                {
                    UserId = groupRequest.SenderId,
                    GroupId = groupRequest.GroupId,
                });

            await _dbContext.SaveChangesAsync();
            await _notifyService.OnGroupIncreased(groupRequest.SenderId, (CommonModels.Group)group);

            return ApiResult.CreateOk();
        }

        [HttpPost("RejectGroupRequest")]
        public async Task<ApiResult> RejectGroupRequestAsync(AcceptRequestRequestData request)
        {
            var selfId = HttpContext.GetUserId();
            var groupRequest = await _dbContext.GroupRequests
                .FirstOrDefaultAsync(gr => gr.Id == request.RequestId);

            if (groupRequest is null)
            {
                return ApiResult.CreateErr("No such group request");
            }

            if (groupRequest.IsDone)
            {
                return ApiResult.CreateErr("This request has already been processed");
            }

            var group = await _dbContext.Groups
                .AsNoTracking()
                .FirstAsync(g => g.Id == groupRequest.GroupId);

            if (group.OwnerId != selfId)
            {
                return ApiResult.CreateErr("You are not owner of that group");
            }

            groupRequest.IsDone = true;

            //await _dbContext.GroupMembers.AddAsync(
            //    new GroupMember()
            //    {
            //        UserId = groupRequest.SenderId,
            //        GroupId = groupRequest.GroupId,
            //    });

            await _dbContext.SaveChangesAsync();
            //await _notifyService.OnGroupIncreased(groupRequest.SenderId, (CommonModels.Group)group);

            return ApiResult.CreateOk();
        }
    }
}

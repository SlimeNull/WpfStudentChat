using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Models.Network;
using StudentChat.Server.Models.Database;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ManageController : ControllerBase
{
    private readonly ChatServerDbContext _dbContext;

    public ManageController(ChatServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("GetUsers")]
    public async Task<ApiResult<GetUsersResultData>> GetUsers(GetUsersRequestData request)
    {
        int count = request.Count;
        if (count < 0)
            count = 1;

        int skip = request.Skip;
        if (skip < 0)
            skip = 0;

        var queryable = _dbContext.Users.AsQueryable();
        if (!string.IsNullOrEmpty(request.NicknameKeyword))
            queryable = queryable.Where(user => user.Nickname.Contains(request.NicknameKeyword));
        if (!string.IsNullOrEmpty(request.UserNameKeyword))
            queryable = queryable.Where(user => user.UserName.Contains(request.UserNameKeyword));

        var totalCount = await queryable.CountAsync(HttpContext.RequestAborted);

        var users = await queryable
            .Skip(skip)
            .Take(count)
            .Select(user => (CommonModels.User)user)
            .ToArrayAsync(HttpContext.RequestAborted);

        return ApiResult<GetUsersResultData>.CreateOk(new(totalCount, users));
    }

    [HttpPost("AddUser")]
    public async Task<ApiResult<AddUserResultData>> AddUser(AddUserRequestData request)
    {
        var alreadyExist = await _dbContext.Users.AnyAsync(user => user.UserName == request.User.UserName, HttpContext.RequestAborted);
        if (alreadyExist)
        {
            return ApiResult<AddUserResultData>.CreateErr("已经有相同用户名的用户了");
        }

        var user = new User()
        {
            UserName = request.User.UserName,
            PasswordHash = request.PasswordHash,
            Nickname = request.User.Nickname,
        };
        var entry = await _dbContext.Users.AddAsync(user, HttpContext.RequestAborted);

        if (!string.IsNullOrWhiteSpace(request.GroupName))
        {
            var group = _dbContext.Groups.FirstOrDefault(group => group.Name == request.GroupName);
            if (group is null)
            {
                group = new Group()
                {
                    Name = request.GroupName,
                    Owner = user,
                };
                _dbContext.Groups.Add(group);
            }
            group.Members.Add(user);
        }

        await _dbContext.SaveChangesAsync(HttpContext.RequestAborted);

        return ApiResult<AddUserResultData>.CreateOk(
            new AddUserResultData((CommonModels.User)entry.Entity));
    }

    [HttpPost("UpdateUserInfo")]
    public async Task<ApiResult> UpdateUserInfo(UpdateUserInfoRequestData request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(HttpContext.RequestAborted);
        if (user == null)
            return ApiResult.CreateErr("用户不存在");

        user.Nickname = request.User.Nickname;
        user.Bio = request.User.Bio;
        user.AvatarHash = request.User.AvatarHash;
        user.UserName = request.User.UserName;

        if (request.PasswordHash is { })
            user.PasswordHash = request.PasswordHash;

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync(HttpContext.RequestAborted);

        return ApiResult.CreateOk();
    }

    [HttpPost("DeleteUser")]
    public async Task<ApiResult> DeleteUser(DeleteUserRequestData request)
    {
        var user = await _dbContext.Users
            .Include(user => user.OwnedGroups)
            .Include(user => user.JoindGroups)
            .Include(user => user.AcceptedFriends)
            .Include(user => user.AddedFriends)
            .Include(user => user.ReceivedFriendRequests)
            .Include(user => user.ReceivedPrivateMessages)
            .Include(user => user.SentFriendRequests)
            .Include(user => user.SentGroupMessages)
            .Include(user => user.SentPrivateMessages)
            .FirstOrDefaultAsync(user => user.Id == request.UserId, HttpContext.RequestAborted);

        if (user == null)
            return ApiResult.CreateErr("用户不存在");

        user.JoindGroups.Clear();
        user.AcceptedFriends.Clear();
        user.AddedFriends.Clear();
        user.ReceivedFriendRequests.Clear();
        user.ReceivedPrivateMessages.Clear();
        user.SentFriendRequests.Clear();
        user.SentGroupMessages.Clear();
        user.SentPrivateMessages.Clear();
        user.JoindGroups.Clear();
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(HttpContext.RequestAborted);

        return ApiResult.CreateOk();
    }
}

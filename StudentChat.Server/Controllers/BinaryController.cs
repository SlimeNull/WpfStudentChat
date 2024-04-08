using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentChat.Models.Network;
using StudentChat.Server.Models;
using StudentChat.Server.Models.Database;

namespace StudentChat.Server.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class BinaryController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;

        public BinaryController(ChatServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("UploadImage")]
        public async Task<ApiResult<BinaryUploadResultData>> UploadImage(IFormFile file)
        {
            if (file.Length > (1 << 10 << 10))
            {
                // cannot greator than 1mb
                return ApiResult<BinaryUploadResultData>.CreateErr("太大了");
            }

            var fileStream = file.OpenReadStream();
            var ms = new MemoryStream();

            await fileStream.CopyToAsync(ms);

            var bytes = ms.ToArray();
            var hash = SHA256.HashData(bytes);
            var hashText = Convert.ToHexString(hash);

            if (await _dbContext.Images.AnyAsync(image => image.Hash == hashText))
            {
                return ApiResult<BinaryUploadResultData>.CreateOk(new BinaryUploadResultData(hashText));
            }

            await _dbContext.Images.AddAsync(
                new ImageBinary()
                {
                    Hash = hashText,
                    ContentType = file.ContentType,
                    Data = bytes,
                });

            await _dbContext.SaveChangesAsync();

            return ApiResult<BinaryUploadResultData>.CreateOk(new BinaryUploadResultData(hashText));
        }

        [HttpPost("UploadFile")]
        public async Task<ApiResult<BinaryUploadResultData>> UploadFile(IFormFile file)
        {
            if (file.Length > 10 << 10 << 10)
            {
                // cannot greator than 10mb
                return ApiResult<BinaryUploadResultData>.CreateErr("太大了");
            }

            var fileStream = file.OpenReadStream();
            var ms = new MemoryStream();

            await fileStream.CopyToAsync(ms);

            var bytes = ms.ToArray();
            var hash = SHA256.HashData(bytes);
            var hashText = Convert.ToHexString(hash);

            if (await _dbContext.Files.AnyAsync(file => file.Hash == hashText))
            {
                return ApiResult<BinaryUploadResultData>.CreateOk(new BinaryUploadResultData(hashText));
            }

            await _dbContext.Files.AddAsync(
                new FileBinary()    
                {
                    Hash = hashText,
                    ContentType = file.ContentType,
                    Data = bytes,
                });

            await _dbContext.SaveChangesAsync();

            return ApiResult<BinaryUploadResultData>.CreateOk(new BinaryUploadResultData(hashText));
        }

        [HttpGet("GetImage/{hash}")]
        public async Task<IActionResult> GetImageAsync(string hash)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(image => image.Hash == hash);

            if (image is null || image.Data is null)
            {
                return NotFound();
            }

            var contentType = image.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
                contentType = "image/jpeg";

            return File(image.Data, contentType);
        }

        [HttpGet("GetFile/{hash}")]
        public async Task<IActionResult> GetFileAsync(string hash)
        {
            var file = await _dbContext.Files.FirstOrDefaultAsync(file => file.Hash == hash);

            if (file is null || file.Data is null)
            {
                return NotFound();
            }

            return File(file.Data, "application/octet-stream");
        }
    }
}

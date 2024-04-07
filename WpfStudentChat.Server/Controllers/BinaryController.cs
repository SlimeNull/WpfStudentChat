﻿using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WpfStudentChat.Server.Models;
using WpfStudentChat.Server.Models.Database;

namespace WpfStudentChat.Server.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class BinaryController : ControllerBase
    {
        private readonly ChatServerDbContext _dbContext;

        public record BinaryUploadResultData(string Hash);

        public BinaryController(ChatServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("UploadImage")]
        public async Task<ApiResult<BinaryUploadResultData>> UploadImage(IFormFile file)
        {
            if (file.Length > 1 << 10 << 10)
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

            await _dbContext.Images.AddAsync(
                new ImageBinary()
                {
                    Hash = hashText,
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

            await _dbContext.Files.AddAsync(
                new FileBinary()
                {
                    Hash = hashText,
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

            return File(image.Data, "image/jpeg");
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

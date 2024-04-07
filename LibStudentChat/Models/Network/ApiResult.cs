using System.Diagnostics.CodeAnalysis;

namespace StudentChat.Models.Network
{

    public class ApiResult
    {
        public bool Ok { get; set; }
        public string? Message { get; set; }

        public static ApiResult CreateErr(string? message = null)
        {
            return new ApiResult()
            {
                Ok = false,
                Message = message
            };
        }

        public static ApiResult CreateOk()
        {
            return new ApiResult()
            {
                Ok = true,
            };
        }
    }

    public class ApiResult<T>
    {
        [MemberNotNullWhen(true, nameof(Data))]
        public bool Ok { get; set; }
        public string? Message { get; set; }

        public T? Data { get; set; }

        public static ApiResult<T> CreateErr(string? message = null)
        {
            return new ApiResult<T>()
            {
                Ok = false,
                Message = message,
                Data = default,
            };
        }

        public static ApiResult<T> CreateOk(T data)
        {
            return new ApiResult<T>()
            {
                Ok = true,
                Data = data,
            };
        }
    }
}

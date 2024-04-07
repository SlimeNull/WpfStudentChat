using System.Diagnostics.CodeAnalysis;

namespace WpfStudentChat.Server.Models
{
    public class ApiResult<T>
    {
        [MemberNotNullWhen(true, nameof(Data))]
        public bool Ok { get; set; }
        public string? Message { get; set; }

        public T? Data { get; set; }

        public static ApiResult<T> CreateErr()
        {
            return new ApiResult<T>()
            {
                Ok = false,
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

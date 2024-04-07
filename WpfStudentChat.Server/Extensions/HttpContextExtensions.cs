using System.Security.Claims;

namespace WpfStudentChat.Server.Extensions
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this HttpContext httpContext)
        {
            var claim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (claim is null)
                return -1;
            if (!int.TryParse(claim.Value, out var id))
                return -1;

            return id;
        }
    }
}

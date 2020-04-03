using Microsoft.AspNetCore.Builder;

namespace YouZack.FromJsonBody
{
    public static class FromJsonBodyMiddlewareExtensions
    {
        public static IApplicationBuilder UseFromJsonBody(this IApplicationBuilder appBuilder)
        {
            return appBuilder.UseMiddleware<FromJsonBodyMiddleware>();
        }
    }
}

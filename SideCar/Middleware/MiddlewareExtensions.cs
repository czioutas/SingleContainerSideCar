using Microsoft.AspNetCore.Builder;

namespace SideCar.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseProxy(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ProxyMiddleware>();
        }

        public static IApplicationBuilder UseEndResponse(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EndResponseMiddleware>();
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SideCar.Middleware
{
    public class EndResponseMiddleware
    {
        private readonly RequestDelegate _next;
        public EndResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            return;
        }
    }
}
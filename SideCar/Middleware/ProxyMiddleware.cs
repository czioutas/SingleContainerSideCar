using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SideCar.Settings;

namespace SideCar.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly DriverApplicationSettings _driverAppSettings;

        public ProxyMiddleware(RequestDelegate next, IOptions<DriverApplicationSettings> settings)
        {
            _next = next;
            _driverAppSettings = settings?.Value ?? throw new ArgumentException(nameof(DriverApplicationSettings));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Host.Host == "localhost")
            {
                await Proxy.Outgoing.ProcessRequestAsync(context);
            }
            else
            {
                await Proxy.Incoming.ProcessRequest(context, _driverAppSettings);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
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
        private readonly ProxySettings _proxySettings;

        public ProxyMiddleware(
            RequestDelegate next,
            IOptions<DriverApplicationSettings> driverAppSettings,
            IOptions<ProxySettings> proxySettings)
        {
            _next = next;
            _driverAppSettings = driverAppSettings?.Value ?? throw new ArgumentException(nameof(DriverApplicationSettings));
            _proxySettings = proxySettings?.Value ?? throw new ArgumentException(nameof(ProxySettings));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Host.Port == _proxySettings.InternalPort)
            {
                // await context.Response.WriteAsync("request is coming from inside");
                await Proxy.Outgoing.ProcessRequestAsync(context);
            }
            else
            {
                // await context.Response.WriteAsync("request is coming from outside");
                await Proxy.Incoming.ProcessRequest(context, _driverAppSettings);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
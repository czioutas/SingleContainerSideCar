using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SideCar.Settings;

namespace SideCar.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DriverApplicationSettings _driverAppSettings;
        private readonly ProxySettings _proxySettings;
        private readonly ILogger _logger;

        public ProxyMiddleware(
            RequestDelegate next,
            IOptions<DriverApplicationSettings> driverAppSettings,
            IOptions<ProxySettings> proxySettings,
            ILogger<ProxyMiddleware> logger)

        {
            _next = next;
            _driverAppSettings = driverAppSettings?.Value ?? throw new ArgumentException(nameof(DriverApplicationSettings));
            _proxySettings = proxySettings?.Value ?? throw new ArgumentException(nameof(ProxySettings));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger<ProxyMiddleware>));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Host.Port == _proxySettings.InternalPort)
            {
                _logger.LogInformation(LoggingEvents.ProxyInternalRequest, "Internal Request from {host}" , context.Request.Host);

                await Proxy.Outgoing.ProcessRequestAsync(context);
            }
            else
            {
                _logger.LogInformation(LoggingEvents.ProxyExternalRequest, "External Request from {host}" , context.Request.Host);
                await Proxy.Incoming.ProcessRequest(context, _driverAppSettings);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
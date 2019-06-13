using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SideCar.Services;
using SideCar.Services.Contracts;
using SideCar.Settings;
using SideCar.Extensions;

namespace SideCar.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IIncomingProxyService _incomingProxy;
        private readonly IOutgoingProxyService _outgoingProxy;
        private readonly DriverApplicationSettings _driverAppSettings;
        private readonly SideCarSettings _sideCarSettings;
        private readonly ILogger _logger;

        public ProxyMiddleware(
            RequestDelegate next,
            IIncomingProxyService incomingProxy,
            IOutgoingProxyService outgoingProxy,
            IOptions<DriverApplicationSettings> driverAppSettings,
            IOptions<SideCarSettings> sideCarSettings,
            ILogger<ProxyMiddleware> logger)

        {
            _next = next;
            _incomingProxy = incomingProxy ?? throw new ArgumentException(nameof(IncomingProxyService));
            _outgoingProxy = outgoingProxy ?? throw new ArgumentException(nameof(OutgoingProxyService));
            _driverAppSettings = driverAppSettings?.Value ?? throw new ArgumentException(nameof(DriverApplicationSettings));
            _sideCarSettings = sideCarSettings?.Value ?? throw new ArgumentException(nameof(sideCarSettings));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger<ProxyMiddleware>));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Outgoing Request
            // - is it coming from App?
            // -- is it going outside or to another app
            // --- Service Discovery for URL/IP
            // ---- Send to Result of SD

            // Incoming Request
            // - is it coming from another app?
            // -- redirect to SelfApp
            // - is it coming from outside?
            // -- redirect to SelfApp

            var test = HttpRequestExtenions.IsLocal(context.Request);

            if (context.Request.Host.Port == _sideCarSettings.AppPort)
            {
                _logger.LogInformation(LoggingEvents.ProxyInternalRequest, "Internal Request from {host}", context.Request.Host);

                await _outgoingProxy.ProcessRequestAsync(context);
            }
            else
            {
                _logger.LogInformation(LoggingEvents.ProxyExternalRequest, "External Request from {host}", context.Request.Host);
                await _incomingProxy.ProcessRequestAsync(context, _driverAppSettings);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
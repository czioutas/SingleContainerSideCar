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
using System.Collections.Generic;

namespace SideCar.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IIncomingProxyService _incomingProxy;
        private readonly IOutgoingProxyService _outgoingProxy;
        private readonly SideCarSettings _sideCarSettings;

        private IHttpContextAccessor _accessor;
        private List<string> ips = new List<string> {
            "0.0.0.0",
            "localhost",
            "*",
            "127.0.0.1"
        };
        private readonly ILogger _logger;

        public ProxyMiddleware(
            RequestDelegate next,
            IIncomingProxyService incomingProxy,
            IOutgoingProxyService outgoingProxy,
            IOptions<SideCarSettings> sideCarSettings,
            IHttpContextAccessor accessor,
            ILogger<ProxyMiddleware> logger)

        {
            _next = next;
            _incomingProxy = incomingProxy ?? throw new ArgumentException(nameof(IncomingProxyService));
            _outgoingProxy = outgoingProxy ?? throw new ArgumentException(nameof(OutgoingProxyService));
            _sideCarSettings = sideCarSettings?.Value ?? throw new ArgumentException(nameof(sideCarSettings));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger<ProxyMiddleware>));
            _accessor = accessor;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_accessor.HttpContext.Connection.RemotePort == _sideCarSettings.SelfPort &&
                ips.Contains(_accessor.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                _logger.LogCritical("Internal Loop");
                await _next(context);
            }

            _logger.LogInformation(LoggingEvents.ProxyInternalRequest, "Internal Request from {host}", context.Request.Host);
            await _outgoingProxy.ProcessRequestAsync(context);


            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
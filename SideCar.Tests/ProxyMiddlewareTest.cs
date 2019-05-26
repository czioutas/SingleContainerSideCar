using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SideCar.Services.Contracts;
using Moq;
using SideCar.Middleware;
using Microsoft.AspNetCore.Http;
using SideCar.Settings;
using Microsoft.Extensions.Options;

namespace SideCar.Tests
{
    [TestClass]
    public class ProxyMiddleware_InvokeAsyncShould
    {
        private readonly ILogger _logger;

        private IIncomingProxyService _incomingProxy;

        private IOutgoingProxyService _outgoingProxy;

        private ProxyMiddleware _proxyMiddleware;

        private RequestDelegate _next;

        private DriverApplicationSettings _driverAppSettings;

        private ProxySettings _proxySettings;

        public ProxyMiddleware_InvokeAsyncShould()
        {
            _incomingProxy = new Mock<IIncomingProxyService>().Object;
            _outgoingProxy = new Mock<IOutgoingProxyService>().Object;

            _proxyMiddleware = new ProxyMiddleware(
                _next,
                _incomingProxy,
                _outgoingProxy,
                new Mock<IOptions<DriverApplicationSettings>>().Object,
                new Mock<IOptions<ProxySettings>>().Object,
                new Mock<ILogger<ProxyMiddleware>>().Object
                );
        }
    }
}
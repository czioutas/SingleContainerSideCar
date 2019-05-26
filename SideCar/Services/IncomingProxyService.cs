using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SideCar.Services.Contracts;
using SideCar.Settings;

namespace SideCar.Services
{
    public class IncomingProxyService : IIncomingProxyService
    {
        private readonly HttpClient _httpClient;

        public IncomingProxyService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentException(nameof(HttpClient));
        }

        public async Task ProcessRequestAsync(HttpContext context, DriverApplicationSettings settings)
        {
            try
            {
                var url = "http://localhost:" + settings.Port + context.Request.Path.Value;
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                context.Response.StatusCode = 200;

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                var a = e;
                var b = e.Message;
            }
        }
    }
}
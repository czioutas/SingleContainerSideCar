using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SideCar.Models;
using SideCar.Services.Contracts;

namespace SideCar.Services
{
    public class OutgoingProxyService : IOutgoingProxyService
    {
        private readonly HttpClient _httpClient;

        public OutgoingProxyService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentException(nameof(HttpClient));
        }

        private async Task ArbitaryRulesForDevSake(HttpContext context)
        {
            if (context.Request.Method != "GET")
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Currently we only handle GET requests 😢");
                return;
            }
        }

        private async Task<string> ExtractTarget(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("target"))
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Could not determine target url of request. Please provide a header.");
                return null;
            }

            string targetUri = context.Request.Headers["target"];
            context.Request.Headers.Remove("targetUri");

            return targetUri;
        }

        private async Task ForwardRequest(HttpContext context, string targetUri)
        {
            ResponseModel _r = new ResponseModel();
            _r.ProcessedRequest = new MetaData
            {
                Headers = context.Request.Headers
            };

            _r.ProcessedResponse = new MetaData();

            HttpResponseMessage response = await _httpClient.GetAsync(targetUri);
            _r.ProcessedResponse.StatusCode = response.StatusCode.ToString();
            _r.ProcessedResponse.Body = await response.Content.ReadAsStringAsync();

            context.Response.StatusCode = 200;

            var json = JsonConvert.SerializeObject(_r);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

        public async Task ProcessRequestAsync(HttpContext context)
        {
            await ArbitaryRulesForDevSake(context);
            string targetUri = await ExtractTarget(context);

            if (targetUri == null)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Target URI Empty.");
                return;
            }

            await ForwardRequest(context ,targetUri);
        }
    }
}
using System;
using System.Collections.Generic;
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

        private Dictionary<string, string> _sd;

        public OutgoingProxyService(HttpClient httpClient)
        {
            _sd = new Dictionary<string, string> {
                { "app_key","app_resolved_address" }
            };

            _httpClient = httpClient ?? throw new ArgumentException(nameof(HttpClient));
        }

        private string SD(string key)
        {
            if (_sd.ContainsKey(key)) {
                return key;
            }

            return _sd[key];
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
            string targetUri = SD(context.Request.Host.Value);

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
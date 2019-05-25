using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SideCar.Models;

namespace SideCar.Proxy
{
    public static class Outgoing
    {
        public static async Task ProcessRequestAsync(HttpContext context)
        {
            string b = "";

            if (context.Request.Method != "GET")
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Currently we only handle GET requests 😢");    
                return;            
            }

            foreach (var a in context.Request.Headers)
            {
                b += a.Key + "_" + a.Value + Environment.NewLine;
            }

            if (!context.Request.Headers.ContainsKey("internal"))
            {
                await context.Response.WriteAsync("Could not determine if request is Internal or not. Please provide a header");
                return;
            }

            if (!context.Request.Headers.ContainsKey("target"))
            {
                await context.Response.WriteAsync("Could not determine target url of request. Please provide a header");
                return;
            }

            bool isInternal = context.Request.Headers["internal"] == "true" ? true : false;
            context.Request.Headers.Remove("internal");

            string targetUri = context.Request.Headers["target"];
            context.Request.Headers.Remove("targetUri");

            if (targetUri == "ping") {
                await context.Response.WriteAsync("pong");
                return;
            }

            ResponseModel _r = new ResponseModel();
            _r.ProcessedRequest = new MetaData
            {
                Headers = context.Request.Headers
            };

            _r.ProcessedResponse = new MetaData();


            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(targetUri);
                _r.ProcessedResponse.StatusCode = response.StatusCode.ToString();
                _r.ProcessedResponse.Body = await response.Content.ReadAsStringAsync();
            }

            context.Response.StatusCode = 200;

            var json = JsonConvert.SerializeObject(_r);
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);
        }
    }
}
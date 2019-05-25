using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SideCar.Models;
using SideCar.Settings;

namespace SideCar.Proxy
{
    public static class Incoming
    {
        public static async Task ProcessRequest(HttpContext context, DriverApplicationSettings settings)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var url = "http://localhost:" + settings.Port + context.Request.Path.Value;
                    HttpResponseMessage response = await client.GetAsync(url);

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
}
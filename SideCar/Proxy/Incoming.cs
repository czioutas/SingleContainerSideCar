using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SideCar.Settings;

namespace SideCar.Proxy
{
    public static class Incoming
    {
        public static async Task ProcessRequest(HttpContext context, DriverApplicationSettings settings)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("localhost:" + settings.Port);
                // _r.ProcessedResponse.StatusCode = response.StatusCode.ToString();
                // _r.ProcessedResponse.Body = await response.Content.ReadAsStringAsync();
            }
        }        
    }
}
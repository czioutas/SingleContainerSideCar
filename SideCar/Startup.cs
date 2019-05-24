using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SideCar.Models;

namespace SideCar
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.Map("", DefaultRoute);
        }

        private static void DefaultRoute(IApplicationBuilder app)
        {
            app.Run(async context => {
                string b = "";                             
                
                if (context.Request.Method != "GET") {
                    await context.Response.WriteAsync("Currently we only handle GET requests 😢");
                    return;
                }

                foreach(var a in context.Request.Headers) {
                    b += a.Key + "_" + a.Value + Environment.NewLine;
                }

                if (!context.Request.Headers.ContainsKey("internal")) {
                    await context.Response.WriteAsync("Could not determine if request is Internal or not. Please provide a header");
                    return;
                }

                if (!context.Request.Headers.ContainsKey("target")) {
                    await context.Response.WriteAsync("Could not determine target url of request. Please provide a header");
                    return;                    
                }
                
                bool isInternal = context.Request.Headers["internal"] == "true" ? true : false;
                context.Request.Headers.Remove("internal");

                string targetUri = context.Request.Headers["target"];
                context.Request.Headers.Remove("targetUri");

                ResponseModel _r = new ResponseModel();
                _r.ProcessedRequest = new MetaData {
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
                await context.Response.WriteAsync(json);
            });
        }
    }
}

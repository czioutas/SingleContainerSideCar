using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SideCar.Middleware;
using SideCar.Services;
using SideCar.Services.Contracts;

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
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            StartupExtensions.AutoDiscover(services, Configuration);

            services.AddHttpClient<IIncomingProxyService, IncomingProxyService>()
                 .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddHttpClient<IOutgoingProxyService, OutgoingProxyService>()
                 .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            // services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.MapWhen(
            //     context => !context.Request.Path.ToString().EndsWith("dashboard") && context.Request.Host.Port != 5000,
            //     appBuilder =>
            // {

            // });

            app.UseProxy();
            app.UseEndResponse();

            // var routeBuilder = new RouteBuilder(app);

            // routeBuilder.MapRoute("dashboard", context =>
            // {
            //     var path = System.IO.Path.Combine(env.ContentRootPath, "file.txt");
            //     FileStream fileStream = new FileStream( path, FileMode.Open);
            //     string data;
            //     using (StreamReader reader = new StreamReader(fileStream))
            //     {
            //         data = reader.ReadToEnd();
            //     }
            //     return context.Response.WriteAsync(data);
            // });

            // var routes = routeBuilder.Build();
            // app.UseRouter(routes);
        }
    }
}
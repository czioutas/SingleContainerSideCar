using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SideCar.Settings;

namespace SideCar
{
    public static class StartupExtensions
    {
        public static void AutoDiscover(IServiceCollection services, IConfiguration configuration)
        {
            SetupAPIs(services);
            SetupServices(services);
            SetupSettings(services, configuration);    
            SetupFilters(services);    
        }

        private static void SetupAPIs(IServiceCollection services)
        {
            var allApi = Assembly.GetEntryAssembly().GetTypes().
                                Where(a => a.GetTypeInfo().IsClass &&
                                        a.Namespace != null &&
                                        a.Namespace.Contains(".APIs") &&
                                        a.Name.EndsWith("APIClient"));

            foreach(Type apiClient in allApi)
            {
                services.AddScoped(apiClient);
            }
        }

        private static void SetupServices(IServiceCollection services)
        {
            IEnumerable<Type> allServices = Assembly.GetEntryAssembly().GetTypes()
                .Where(a => a.GetTypeInfo().IsClass &&
                    a.Namespace != null &&
                    a.Namespace.Contains("SideCar.Services") &&
                    a.Name.EndsWith("Service")
                );          
            
            foreach(Type service in allServices)
            {
                if (service.GetInterfaces().Count() > 0) {
                    services.AddScoped(service.GetInterfaces().First(), service);
                } else {
                    services.AddScoped(service);
                }
            }
        }

        private static void SetupSettings(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DriverApplicationSettings>(configuration.GetSection(nameof(DriverApplicationSettings)));
        }

        private static void SetupFilters(IServiceCollection services)
        {
        
        }
    }
}
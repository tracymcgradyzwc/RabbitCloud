﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.Cloud.Discovery.Abstractions;

namespace Rabbit.Cloud.Discovery.Configuration
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddConfigurationDiscovery(this IServiceCollection services, IConfiguration configuration, string sectionKey = "Instances")
        {
            return services.AddSingleton<IDiscoveryClient>(new ConfigurationDiscoveryClient(configuration.GetSection(sectionKey)));
        }
    }
}
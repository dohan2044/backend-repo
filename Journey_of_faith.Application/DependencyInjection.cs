using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Journey_of_faith.Application.behaviors;
using Journey_of_faith.Application.common.caching;
using Journey_of_faith.Application.common.caching.interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Journey_of_faith.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(ApplicationAssembly).Assembly);
                cfg.AddOpenBehavior(typeof(LoggingRequestBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviors<,>));
                cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
                cfg.AddOpenBehavior(typeof(CacheInvalidBehavior<,>));
            });

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 10000; // Set a size limit for the cache
                options.CompactionPercentage = 0.1;
            });
            services.AddSingleton<ICacheGroupVersionStore, CacheGroupVersionStore>();
            return services;
        }
    }
}

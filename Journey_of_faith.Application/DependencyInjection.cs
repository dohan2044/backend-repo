using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Journey_of_faith.Application.behaviors;
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

            services.AddMemoryCache();
            return services;
        }
    }
}

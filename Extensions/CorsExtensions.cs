using KKHCleanBus.MicroServices.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KKHCleanBus.MicroServices.Extensions
{
    public static class CorsExtensions
    {
        public static void AddCorsSetting(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetCorsConfig();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        if (config.Origins != null && config.Origins.Length > 0)
                            builder.WithOrigins(config.Origins);

                        if (config.Headers != null && config.Headers.Length > 0)
                            builder.WithHeaders(config.Headers);

                        if (config.Methods != null && config.Methods.Length > 0)
                            builder.WithMethods(config.Methods);
                    });
            });
        }

        public static void UseCorsSetting(this WebApplication app)
        {
            app.UseCors();
        }

        public static CorsConfig GetCorsConfig(this IConfiguration configuration)
        {
            var section = configuration.GetSection("CorsConfig");
            var cfg = new CorsConfig
            {
                Origins = section.GetSection("Origins").Get<string[]>(),
                Headers = section.GetSection("Headers").Get<string[]>(),
                Methods = section.GetSection("Methods").Get<string[]>()
            };

            if (cfg.Origins == null || cfg.Origins.Length == 0)
                throw new InvalidOperationException("CorsConfig: 'Origins' must be provided and contain at least one origin.");

            if (cfg.Methods == null || cfg.Methods.Length == 0)
                throw new InvalidOperationException("CorsConfig: 'Methods' must be provided and contain at least one HTTP method.");

            if (cfg.Headers == null || cfg.Headers.Length == 0)
                throw new InvalidOperationException("CorsConfig: 'Headers' must be provided and contain at least one header name.");

            return cfg;
        }
    }
}

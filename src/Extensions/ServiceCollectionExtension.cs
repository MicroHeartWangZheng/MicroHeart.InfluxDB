using InfluxDB.Client;
using MicroHeart.InfluxDB.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace MicroHeart.InfluxDB.Extentsions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfluxDB(this IServiceCollection services, Action<InfluxDBOptions> configAction, ServiceLifetime lifetime = ServiceLifetime.Singleton)
             => AddInfluxDB(services, (serviceProvider, options) => configAction.Invoke(options), lifetime);

        public static IServiceCollection AddInfluxDB(this IServiceCollection services, IConfiguration config, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            services.Configure<InfluxDBOptions>(config);

            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddSingleton<InfluxDBClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<InfluxDBOptions>>();

                        return new InfluxDBClient(Mapping(options.Value));
                    });
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<InfluxDBClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<InfluxDBOptions>>();
                        return new InfluxDBClient(Mapping(options.Value));
                    });
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<InfluxDBClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<InfluxDBOptions>>();
                        return new InfluxDBClient(Mapping(options.Value));
                    });
                    break;
            }

            return services;
        }


        public static IServiceCollection AddInfluxDB(this IServiceCollection services, Action<IServiceProvider, InfluxDBOptions> configAction, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configAction == null)
                throw new ArgumentNullException(nameof(configAction));

            services.AddOptions<InfluxDBOptions>()
                    .Configure<IServiceProvider>((options, sp) => configAction(sp, options));

            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddSingleton<InfluxDBClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<InfluxDBOptions>>();
                        return new InfluxDBClient(Mapping(options.Value));
                    });
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<InfluxDBClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<InfluxDBOptions>>();
                        return new InfluxDBClient(Mapping(options.Value));
                    });
                    break;

                case ServiceLifetime.Singleton:
                    services.AddSingleton<InfluxDBClient>(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<InfluxDBOptions>>();
                        return new InfluxDBClient(Mapping(options.Value));
                    });
                    break;
            }

            return services;
        }


        private static InfluxDBClientOptions Mapping(InfluxDBOptions options)
        {
            return new InfluxDBClientOptions(options.Url)
            {
                Timeout = options.Timeout,
                LogLevel = options.LogLevel,
                Token = options.Token,
                Org = options.Org,
                Bucket = options.Bucket,
            };
        }
    }
}

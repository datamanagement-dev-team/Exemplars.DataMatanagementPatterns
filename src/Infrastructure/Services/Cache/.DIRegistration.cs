using Autofac;
using BlueBrown.Common.HealthChecks.Redis.Configuration;
using BlueBrown.Common.HealthChecks.Redis.Extensions;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Cache;
using BlueBrown.Diagnostics.HealthChecks.DependencyInjection;
using CommonHealthChecks.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Cache
{
	internal static class DIRegistration
	{
		internal static void RegisterCache(this IServiceCollection services, IConfiguration configuration)
		{
			var redisConnectionSettings = configuration.GetSection(RedisConnectionSettings.ConfigurationKey).Get<RedisConnectionSettings>()!;

			var settings = configuration.GetSection(CacheSettings.ConfigurationKey).Get<CacheSettings>()!;

			var connectionStringTemplate = settings.ConnectionStringTemplate;
			var connectionString = string.Format(
				connectionStringTemplate,
				redisConnectionSettings.Endpoints,
				redisConnectionSettings.Password,
				redisConnectionSettings.ServiceName);
			settings.SetConnectionString(connectionString);

			var errors = settings.Validate();

			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			var configurationOptions = ConfigurationOptions.Parse(settings.ConnectionString);
			var connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
			services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

			services
				.AddHealthChecksCustom()
				.AddCheckRedis(
					name: "Redis",
					redisConfiguration: new RedisConfiguration
					{
						ServiceName = redisConnectionSettings.ServiceName,
						Password = redisConnectionSettings.Password,
						EndPoints = redisConnectionSettings.Endpoints.Split(",").ToList()
					},
					healthCheckPriority: HealthCheckPriority.Critical,
					healthCheckProbe: HealthCheckProbe.Ready,
					tags: new string[] { }
				);
		}

		internal static void RegisterCache(this ContainerBuilder builder)
		{
			builder
				.RegisterType<Cache>()
				.As<ICache>()
				.SingleInstance();
		}
	}
}

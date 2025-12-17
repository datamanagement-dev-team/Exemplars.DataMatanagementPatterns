using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Repository;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Resilience
{
	internal static class DIRegistration
	{
		internal static void RegisterResilienceService(this ContainerBuilder builder)
		{
			builder
				.Register(componentContext =>
				{
					var settings = componentContext.Resolve<IAccountHttpClientSettings>();
					var logger = componentContext.Resolve<ILogger<ResilienceService>>();

					var resilienceService = new ResilienceService(logger);
					if (settings.Resilience?.Retry is not null)
						resilienceService.AddRetry(settings.Resilience.Retry);
					if (settings.Resilience?.Timeout is not null)
						resilienceService.AddTimeout(settings.Resilience.Timeout);

					return resilienceService;
				})
				.Keyed<IResilienceService>(nameof(IAccountHttpClient))
				.InstancePerLifetimeScope();

			builder
				.Register(componentContext =>
				{
					var settings = componentContext.Resolve<IRepositorySettings>();
					var logger = componentContext.Resolve<ILogger<ResilienceService>>();

					var resilienceService = new ResilienceService(logger);
					resilienceService.AddRetry(settings.Resilience.Retry!);
					resilienceService.AddTimeout(settings.Resilience.Timeout!);

					return resilienceService;
				})
				.Keyed<IResilienceService>(nameof(IRepository))
				.InstancePerLifetimeScope();
		}
	}
}

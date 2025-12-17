using Autofac;
using BlueBrown.ActivityPublishers.Shared.Consumers;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Cache;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Metrics;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.OpenTelemetry;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Readiness;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure
{
	public static class DIRegistration
	{
		public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.RegisterConsul(configuration);
			services.RegisterMetrics(configuration);
			services.RegisterOpenTelemetry(configuration);
			services.RegisterReadinessService();
			services.RegisterHttpClients(configuration);
			services.RegisterJobScheduler(configuration);
			services.RegisterKafkaConsumers();
			services.RegisterCache(configuration);
		}
	}

	public class InfrastructureDIRegistrationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterMetrics();
			builder.RegisterHttpClients();
			builder.RegisterResilienceService();
			builder.RegisterJobScheduler();
			builder.RegisterKafkaConsumers();
			builder.RegisterCache();
		}
	}
}

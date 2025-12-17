using Autofac;
using BlueBrown.ActivityPublishers.Shared.Consumers;
using BlueBrown.ActivityPublishers.Shared.Consumers.CasinoRounds;
using BlueBrown.ActivityPublishers.Shared.Consumers.Interfaces;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers.CasinoRound;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers.CasinoRound.Decorators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers
{
	internal static class DIRegistration
	{
		internal static void RegisterKafkaConsumers(this IServiceCollection services)
		{
			using var serviceProvider = services.BuildServiceProvider();

			var configuration = serviceProvider.GetRequiredService<IConfiguration>();

			var kafkaInfrastructureSettings = configuration.GetSection(KafkaInfrastructureSettings.ConfigurationKey).Get<KafkaInfrastructureSettings>()!;
			var errors = kafkaInfrastructureSettings.Validate();

			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			var casinoRoundKafkaSettings = configuration.GetSection(CasinoRoundKafkaSettings.ConfigurationKey).Get<CasinoRoundKafkaSettings>()!;
			errors = casinoRoundKafkaSettings.Validate();

			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			services.AddSingleton(casinoRoundKafkaSettings);

			services.RegisterKafkaConsumers(_configurator =>
			{
				_configurator.WithSettings(kafkaInfrastructureSettings);

				if (casinoRoundKafkaSettings.Enable)
				{
					_configurator.AddCasinoRoundConsumer<CasinoRoundKafkaConsumer>(
						casinoRoundKafkaSettings,
						_configurator =>
						{
							_configurator.ApplyExactlyOnce<TopicPartitionOffsetRepository>();
						});
				}
			});
		}

		internal static void RegisterKafkaConsumers(this ContainerBuilder builder)
		{
			builder
				.RegisterType<TopicPartitionOffsetRepository>()
				.As<ITopicPartitionOffsetRepository>()
				.InstancePerLifetimeScope();

			builder.RegisterDecorator<CasinoRoundKafkaConsumerLogAndMetricsDecorator, ICasinoRoundKafkaConsumer>(_context =>
			{
				var settings = _context.Resolve<CasinoRoundKafkaSettings>();
				return settings.Enable;
			});
		}
	}
}

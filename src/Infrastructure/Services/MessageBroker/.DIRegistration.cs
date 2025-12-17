using BlueBrown.Common.HealthChecks.RabbitMQ.Configuration;
using BlueBrown.Common.HealthChecks.RabbitMQ.Extensions;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.MessageBroker.Consumers;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.MessageBroker.Filters;
using BlueBrown.Diagnostics.HealthChecks.DependencyInjection;
using CommonHealthChecks.Enums;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.MessageBroker
{
	internal static class DIRegistration
	{
		internal static void RegisterMessageBroker(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetRequiredSection(MessageBrokerSettings.ConfigurationKey).Get<MessageBrokerSettings>()!;

			var errors = settings.Validate();
			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			services.AddMassTransit(_configurator =>
			{
				if (settings.Enabled)
				{
					_configurator.SetEndpointNameFormatter(new DefaultEndpointNameFormatter("DataManagementPatterns-"));

					_configurator.AddConsumer<RegistrationMessageBrokerConsumer>();
				}

				_configurator.UsingRabbitMq((_context, _configurator) =>
				{
					_configurator.Host(
						settings.Endpoint, 
						_configurator =>
						{
							_configurator.Username(settings.Username);
							_configurator.Password(settings.Password);
						});

					_configurator.UseConsumeFilter<LogAndMetricsFilter<RegistrationMessageBrokerConsumer>>(_context);

					_configurator.UseNewtonsoftJsonDeserializer();
					_configurator.UseNewtonsoftJsonSerializer();
					_configurator.ConfigureNewtonsoftJsonSerializer(_options =>
					{
						_options.DateTimeZoneHandling = DateTimeZoneHandling.Local;
						return _options;
					});

					_configurator.ConfigureEndpoints(_context);
				});
			});

			var uri = new Uri(settings.Endpoint);

			services
				.AddHealthChecksCustom()
				.AddCheckRabbitMq(
					name: "RabbitMQ",
					rabbitMqConfiguration: new RabbitMqConfiguration
					{
						HostName = uri.Host,
						VirtualHost = uri.AbsolutePath.Trim('/'),
						Username = settings.Username,
						Password = settings.Password,
					},
					healthCheckPriority: HealthCheckPriority.Critical,
					healthCheckProbe: HealthCheckProbe.Ready,
					tags: new string[] { });
		}
	}
}

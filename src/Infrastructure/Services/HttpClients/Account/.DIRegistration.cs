using BlueBrown.Common.HealthChecks.Endpoint.Configuration;
using BlueBrown.Common.HealthChecks.Endpoint.Extensions;
using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.HttpMessageHandlers;
using BlueBrown.Diagnostics.HealthChecks.DependencyInjection;
using CommonHealthChecks.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Account
{
	internal static class DIRegistration
	{
		internal static void RegisterAccountHttpClient(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(AccountHttpClientSettings.ConfigurationKey).Get<AccountHttpClientSettings>()!;

			services.AddSingleton<IAccountHttpClientSettings>(settings);
			services.AddSingleton<IValidatable>(settings);

			services
				.AddHttpClient<IAccountHttpClient, AccountHttpClient>()
				.AddHttpMessageHandler<CorrelationHttpMessageHandler>()
				.AddHttpMessageHandler<LoggingHttpMessageHandler>();

			services
				.AddHealthChecksCustom()
				.AddCheckEndpoint(
					name: nameof(IAccountHttpClient),
					endpointConfiguration: new EndpointConfiguration
					{
						EndpointUrl = $"{settings.BaseUrl}/{settings.HealthCheckEndpoint}",
						RequestMethod = RequestMethod.GET,
						RetryLimit = RetryLimit.Two,
						StatusCodes = new List<int>
						{
							200
						}
					},
					healthCheckPriority: HealthCheckPriority.Critical,
					healthCheckProbe: HealthCheckProbe.Ready,
					tags: new string[] { });
		}
	}
}

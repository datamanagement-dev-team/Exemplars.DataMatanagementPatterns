using BlueBrown.Data.DataManagementPatterns.Application;
using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Data.Common;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.OpenTelemetry
{
	public static class DIRegistration
	{
		public static void ConfigureOpenTelemetrySettings(this IConfigurationBuilder builder)
		{
			builder.AddInMemoryCollection([
				new KeyValuePair<string, string?>("OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_DISABLE_URL_QUERY_REDACTION", "true"),
				new KeyValuePair<string, string?>("OTEL_DOTNET_EXPERIMENTAL_HTTPCLIENT_DISABLE_URL_QUERY_REDACTION", "true")
			]);
		}

		internal static void RegisterOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(OpenTelemetrySettings.ConfigurationKey).Get<OpenTelemetrySettings>()!;

			var errors = settings.Validate();

			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			services
				.AddOpenTelemetry()
				.ConfigureResource(_builder =>
				{
					_builder.
						AddService(
							serviceName: "services.data.datamanagementpatterns",
							serviceNamespace: "reporting",
							serviceVersion: settings.ServiceVersion,
							serviceInstanceId: Environment.MachineName);
				})
				.WithTracing(_builder =>
				{
					_builder.AddAspNetCoreInstrumentation(_options =>
					{
						_options.RecordException = true;
						_options.Filter = _httpContext => !Application.Extensions.IsSystemicEndpoint(_httpContext.Request.Path);
					});

					_builder.AddSqlClientInstrumentation(_options =>
					{
						_options.RecordException = true;
						_options.EnrichWithSqlCommand = (activity, rawObject) =>
						{
							if (rawObject is DbCommand cmd)
							{
								foreach (DbParameter p in cmd.Parameters)
								{
									activity?.SetTag($"db.query.parameter.{p.ParameterName}", p.Value?.ToString());
								}
							}
						};
					});

					_builder.AddHttpClientInstrumentation(_options =>
					{
						_options.EnrichWithHttpRequestMessage = (_activity, _httpRequestMessage) =>
						{
							if (_httpRequestMessage.Headers.Any(_header => _header.Key == LoggingConstants.CorrelationId))
							{
								var correlationId = _httpRequestMessage.Headers.Single(_header => _header.Key == LoggingConstants.CorrelationId).Value.Single();
								_activity.SetTag(LoggingConstants.CorrelationId, correlationId);
							}
						};
						_options.RecordException = true;
					});

					_builder.AddQuartzInstrumentation(_options =>
					{
						_options.RecordException = true;
					});

					_builder.AddSource(DiagnosticHeaders.DefaultListenerName);

					using var serviceProvider = services.BuildServiceProvider();
					var connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();

					_builder.AddRedisInstrumentation(
						connectionMultiplexer,
						_options =>
						{
							_options.EnrichActivityWithTimingEvents = true;
							_options.SetVerboseDatabaseStatements = true;
						});

					_builder.AddOtlpExporter(_options =>
					{
						_options.Endpoint = new Uri(settings.Url);
					});
				});
		}
	}
}

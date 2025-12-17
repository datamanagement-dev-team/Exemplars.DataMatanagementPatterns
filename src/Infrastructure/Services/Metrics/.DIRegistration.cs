using App.Metrics.Formatters.Prometheus;
using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Metrics
{
	public static class DIRegistration
	{
		internal static void RegisterMetrics(this ContainerBuilder builder)
		{
			builder
				.RegisterType<Metrics>()
				.As<IMetrics>()
				.SingleInstance();
		}

		public static void RegisterMetrics(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(MetricsSettings.ConfigurationKey).Get<MetricsSettings>()!;

			services.AddSingleton<IMetricsSettings>(settings);
			services.AddSingleton<IValidatable>(settings);

			AppMetricsServiceCollectionExtensions.AddMetrics(services);
		}

		public static void ConfigureMetrics(this IHostBuilder builder, IConfiguration configuration)
		{
			builder.ConfigureMetrics(_builder =>
			{
				_builder.Configuration.Configure(_options =>
				{
					_options.ContextualTags.Clear();

					_options.GlobalTags.Clear();
				});
			});

			builder.UseMetricsEndpoints(_options =>
			{
				_options.EnvironmentInfoEndpointEnabled = false;

				_options.MetricsEndpointEnabled = true;

				_options.MetricsTextEndpointEnabled = false;

				var options = new MetricsPrometheusOptions
				{
					MetricNameFormatter = (_metricName, _metricTypeName) =>
					{
						_metricName = _metricName.ToLowerInvariant();
						_metricTypeName = _metricTypeName.ToLowerInvariant().Replace(" ", "_");

						var metricName = _metricName.Replace("application.", "", StringComparison.InvariantCultureIgnoreCase);

						metricName = metricName.Replace("application", "", StringComparison.InvariantCultureIgnoreCase);

						if (string.IsNullOrWhiteSpace(metricName))
							return _metricTypeName;
						else
							return $"{metricName}_{_metricTypeName}";
					}
				};
				_options.MetricsEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter(options);
			});

			builder.ConfigureAppMetricsHostingConfiguration(_options =>
			{
				var settings = new MetricsSettings();

				configuration.Bind(MetricsSettings.ConfigurationKey, settings);

				_options.MetricsEndpoint = settings.Url;
			});

			builder.UseMetricsWebTracking();
		}

		public static void UseMetrics(this IApplicationBuilder builder)
		{
			builder.UseMetricsAllMiddleware();
		}
	}
}

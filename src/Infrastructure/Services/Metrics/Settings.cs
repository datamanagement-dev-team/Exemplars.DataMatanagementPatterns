using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Metrics
{
	internal record MetricsSettings : IMetricsSettings
	{
		public const string ConfigurationKey = "metricsconfiguration";

		public string Url { get; init; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(Url))
				errors.Add($"{nameof(IMetricsSettings.Url)} should not be null");

			if (!Url.StartsWith("/"))
				errors.Add($"{nameof(IMetricsSettings.Url)} should start with /");

			return errors;
		}
	}
}

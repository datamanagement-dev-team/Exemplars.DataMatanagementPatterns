using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.OpenTelemetry
{
	internal class OpenTelemetrySettings : ISettings
	{
		public const string ConfigurationKey = "opentelemetryconfiguration";

		public string ServiceVersion { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(ServiceVersion))
				errors.Add($"{nameof(ServiceVersion)} should not be null");

			if (string.IsNullOrWhiteSpace(Url))
				errors.Add($"{nameof(Url)} should not be null");

			return errors;
		}
	}
}

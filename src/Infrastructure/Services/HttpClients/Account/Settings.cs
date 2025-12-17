using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Account
{
	internal record AccountHttpClientSettings : IAccountHttpClientSettings
	{
		public const string ConfigurationKey = "accounthttpclientconfiguration";

		public string BaseUrl { get; init; } = string.Empty;
		public string HealthCheckEndpoint { get; init; } = string.Empty;
		public ResilienceSettings? Resilience { get; init; }
		public string Version { get; init; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(BaseUrl))
				errors.Add($"{nameof(IAccountHttpClientSettings.BaseUrl)} should not be null");

			if (string.IsNullOrWhiteSpace(Version))
				errors.Add($"{nameof(IAccountHttpClientSettings.Version)} should not be null");

			return errors;
		}
	}
}

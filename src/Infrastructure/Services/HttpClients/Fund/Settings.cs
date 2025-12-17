using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Fund
{
	internal record FundHttpClientSettings : IFundHttpClientSettings
	{
		public const string ConfigurationKey = "fundhttpclientconfiguration";

		public string BaseUrl { get; init; } = string.Empty;
		public string HealthCheckEndpoint { get; init; } = string.Empty;
		public ResilienceSettings? Resilience { get; init; }

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(BaseUrl))
				errors.Add($"{nameof(IAccountHttpClientSettings.BaseUrl)} should not be null");

			return errors;
		}
	}
}

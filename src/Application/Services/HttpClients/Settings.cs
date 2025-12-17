using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients
{
	public interface IHttpClientSettings : ISettings
	{
		string BaseUrl { get; }
		string HealthCheckEndpoint { get; }
		ResilienceSettings? Resilience { get; }
	}
}

using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund;
using BlueBrown.Funds.HttpClient;
using BlueBrown.Funds.Shared.Results;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Fund
{
	internal class FundHttpClient : IFundHttpClient
	{
		private readonly IFundsClient _client;
		private readonly HttpClient _httpClient;
		private readonly IFundHttpClientSettings _settings;

		public FundHttpClient(
			HttpClient httpClient,
			IFundHttpClientSettings settings)
		{
			_httpClient = httpClient;
			_client = ClientFactory.CreateFundsClient(settings.BaseUrl, _httpClient);
			_settings = settings;
		}

		public ValueTask DisposeAsync()
		{
			_httpClient.Dispose();
			return ValueTask.CompletedTask;
		}

		public async Task<AvailableFundsResult> GetFunds(long customerId, CancellationToken cancellationToken = default)
		{
			_httpClient.DefaultRequestHeaders.Add(LoggingConstants.Service, nameof(IFundHttpClient));
			_httpClient.DefaultRequestHeaders.Add(LoggingConstants.Method, nameof(GetFunds));
			_httpClient.DefaultRequestHeaders.Add(LoggingConstants.HttpRequestUrl, $"{_settings.BaseUrl}/api/v1/funds/{{customerId}}");

			return await _client.GetAvailableFundsAsync(
				accountId: customerId,
				operation: default,
				operationContext: default,
				sessionId: default,
				cancellationToken: cancellationToken);
		}
	}
}

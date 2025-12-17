using BlueBrown.Accounts.HttpClient;
using BlueBrown.Accounts.Shared.Results.Account;
using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Account
{
	internal class AccountHttpClient : IAccountHttpClient
	{
		private readonly IAccountClient _client;
		private readonly HttpClient _httpClient;
		private readonly IAccountHttpClientSettings _settings;

		public AccountHttpClient(
			HttpClient httpClient,
			IAccountHttpClientSettings settings)
		{
			_httpClient = httpClient;
			_client = ClientFactory.CreateAccountClient(settings.BaseUrl, _httpClient);
			_settings = settings;
		}

		public ValueTask DisposeAsync()
		{
			_httpClient.Dispose();
			return ValueTask.CompletedTask;
		}

		public async Task<GetAccountResponse> GetCustomer(long customerId, CancellationToken cancellationToken = default)
		{
			_httpClient.DefaultRequestHeaders.Add(LoggingConstants.Service, nameof(IAccountHttpClient));
			_httpClient.DefaultRequestHeaders.Add(LoggingConstants.Method, nameof(GetCustomer));
			_httpClient.DefaultRequestHeaders.Add(LoggingConstants.HttpRequestUrl, $"{_settings.BaseUrl}/api/v1/account/get/{{customerId}}");

			return await _client.GetAsync(customerId, _settings.Version, cancellationToken);
		}
	}
}

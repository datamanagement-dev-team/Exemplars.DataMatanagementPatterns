using Autofac.Features.Indexed;
using BlueBrown.Accounts.Shared.Results.Account;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account.Decorators
{
	internal class AccountHttpClientResilienceDecorator : IAccountHttpClient
	{
		private readonly IAccountHttpClient _decorated;
		private readonly IResilienceService _resilienceService;

		public AccountHttpClientResilienceDecorator(
			IAccountHttpClient decorated,
			IIndex<string, IResilienceService> index)
		{
			_decorated = decorated;
			_resilienceService = index[nameof(IAccountHttpClient)];
		}

		public async ValueTask DisposeAsync()
		{
			await _decorated.DisposeAsync();
		}

		public async Task<GetAccountResponse> GetCustomer(long customerId, CancellationToken cancellationToken = default)
		{
			Func<CancellationToken, ValueTask<GetAccountResponse>> func = async _cancellationToken => await _decorated.GetCustomer(customerId, _cancellationToken);
			return await _resilienceService.ExecuteAsync(func, cancellationToken);
		}
	}
}

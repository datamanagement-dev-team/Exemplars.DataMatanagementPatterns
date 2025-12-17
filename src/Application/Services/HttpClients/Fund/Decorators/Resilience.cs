using Autofac.Features.Indexed;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;
using BlueBrown.Funds.Shared.Results;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund.Decorators
{
	internal class FundHttpClientResilienceDecorator : IFundHttpClient
	{
		private readonly IFundHttpClient _decorated;
		private readonly IResilienceService _resilienceService;

		public FundHttpClientResilienceDecorator(
			IFundHttpClient decorated,
			IIndex<string, IResilienceService> index)
		{
			_decorated = decorated;
			_resilienceService = index[nameof(IFundHttpClient)];
		}

		public async ValueTask DisposeAsync()
		{
			await _decorated.DisposeAsync();
		}

		public async Task<AvailableFundsResult> GetFunds(long customerId, CancellationToken cancellationToken = default)
		{
			Func<CancellationToken, ValueTask<AvailableFundsResult>> func = async _cancellationToken => await _decorated.GetFunds(customerId, _cancellationToken);
			return await _resilienceService.ExecuteAsync(func, cancellationToken);
		}
	}
}

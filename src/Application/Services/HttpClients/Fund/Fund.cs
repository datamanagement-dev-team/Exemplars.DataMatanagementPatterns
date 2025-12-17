using BlueBrown.Funds.Shared.Results;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund
{
	public interface IFundHttpClient : IHttpClient
	{
		Task<AvailableFundsResult> GetFunds(long customerId, CancellationToken cancellationToken = default);
	}
}

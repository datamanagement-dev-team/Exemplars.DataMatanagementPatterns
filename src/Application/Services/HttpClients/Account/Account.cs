using BlueBrown.Accounts.Shared.Results.Account;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account
{
	public interface IAccountHttpClient : IHttpClient
	{
		Task<GetAccountResponse> GetCustomer(long customerId, CancellationToken cancellationToken = default);
	}
}

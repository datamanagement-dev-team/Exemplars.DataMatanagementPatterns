using BlueBrown.Data.DataManagementPatterns.Shared.Customer;
using RestSharp;

namespace BlueBrown.Data.DataManagementPatterns.Shared.Clients
{
	public interface ICustomerClient
	{
		Task<Response<GetCustomerResult>> Get(long customerId, CancellationToken cancellationToken = default);
		Task Create(CreateCustomerRequest request, CancellationToken cancellationToken = default);
	}

	internal class CustomerClient : Client, ICustomerClient
	{
		public CustomerClient(
			string baseUrl,
			HttpClient? httpClient = null)
			: base(baseUrl, httpClient)
		{
		}

		public async Task<Response<GetCustomerResult>> Get(long customerId, CancellationToken cancellationToken = default)
		{
			return await ExecuteWithResponse<GetCustomerResult>(
				resource: "/api/customer/get",
				parameters: new Dictionary<string, string>
				{
					{ "customerId", customerId.ToString() }
				},
				cancellationToken: cancellationToken);
		}

		public async Task Create(CreateCustomerRequest request, CancellationToken cancellationToken = default)
		{
			await Execute(
				resource: "/api/customer/create",
				httpMethod: Method.Post,
				request: request,
				cancellationToken: cancellationToken);
		}
	}
}

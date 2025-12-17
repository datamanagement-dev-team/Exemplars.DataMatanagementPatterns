namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Repository
{
	public interface IRepository
	{
		Task<object> GetCustomer(long customerId, CancellationToken cancellationToken = default);
	}
}

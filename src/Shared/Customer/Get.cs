namespace BlueBrown.Data.DataManagementPatterns.Shared.Customer
{
	public record GetCustomerResult
	{
		public long CustomerId { get; }

		public GetCustomerResult(long customerId)
		{
			CustomerId = customerId;
		}
	}
}

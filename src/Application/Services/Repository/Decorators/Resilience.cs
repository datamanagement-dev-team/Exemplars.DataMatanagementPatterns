using Autofac.Features.Indexed;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Repository.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class RepositoryResilienceDecorator : IRepository
	{
		private readonly IRepository _decorated;
		private readonly IResilienceService _resilienceService;

		public RepositoryResilienceDecorator(
			IRepository decorated,
			IIndex<string, IResilienceService> index)
		{
			_decorated = decorated;
			_resilienceService = index[nameof(IRepository)];
		}

		public async Task<object> GetCustomer(long customerId, CancellationToken cancellationToken = default)
		{
			Func<CancellationToken, ValueTask<object>> func = async _cancellationToken => await _decorated.GetCustomer(customerId, _cancellationToken);
			return await _resilienceService.ExecuteAsync(func, cancellationToken);
		}
	}
}

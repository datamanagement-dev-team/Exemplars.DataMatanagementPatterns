
using BlueBrown.Data.DataManagementPatterns.Application.Decorators;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Repository.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class RepositoryLogAndMetricsDecorator : LogAndMetricsDecorator, IRepository
	{
		private readonly IRepository _decorated;

		public RepositoryLogAndMetricsDecorator(
			IRepository decorated,
			IMetrics metrics,
			ILogger<RepositoryLogAndMetricsDecorator> logger)
			: base(
				  contextName: "application",
				  serviceName: nameof(IRepository),
				  metrics: metrics,
				  logger: logger)
		{
			_decorated = decorated;
		}

		public async Task<object> GetCustomer(long customerId, CancellationToken cancellationToken = default)
		{
			Func<Task<object>> func = async () => await _decorated.GetCustomer(customerId, cancellationToken);
			return await Decorate(func, logLevel: LogLevel.Trace);
		}
	}
}

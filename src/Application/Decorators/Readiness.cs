using BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Application.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public abstract class ReadinessDecorator
	{
		private readonly IReadinessService _readinessService;
		private readonly ILogger<ReadinessDecorator> _logger;

		protected ReadinessDecorator(
			IReadinessService readinessService,
			ILogger<ReadinessDecorator> logger)
		{
			_readinessService = readinessService;
			_logger = logger;
		}

		public async Task Decorate(Func<Task> func, CancellationToken cancellationToken = default)
		{
			if (!await (_readinessService.IsReady(cancellationToken)))
			{
				_logger.LogWarning("Service is not ready. Skipping process");

				return;
			}

			await func();
		}
	}
}

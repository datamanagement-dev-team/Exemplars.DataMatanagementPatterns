using BlueBrown.Data.DataManagementPatterns.Application.Decorators;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Processor.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class ProcessorReadinessDecorator : ReadinessDecorator, IProcessor
	{
		private readonly IProcessor _decorated;

		public ProcessorReadinessDecorator(
			IProcessor decorated,
			IReadinessService readinessService,
			ILogger<ReadinessDecorator> logger)
			: base(readinessService, logger)
		{
			_decorated = decorated;
		}

		public async Task Process(CancellationToken cancellationToken = default)
		{
			Func<Task> func = async () => await _decorated.Process(cancellationToken);
			await Decorate(func, cancellationToken);
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Application.Decorators;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Processor.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class ProcessorLogAndMetricsDecorator : LogAndMetricsDecorator, IProcessor
	{
		private readonly IProcessor _decorated;

		public ProcessorLogAndMetricsDecorator(
			IProcessor decorated,
			IMetrics metrics,
			ILogger<ProcessorLogAndMetricsDecorator> logger)
			: base(
				  contextName: "application",
				  serviceName: nameof(IProcessor),
				  metrics: metrics,
				  logger: logger)
		{
			_decorated = decorated;
		}

		public async Task Process(CancellationToken cancellationToken = default)
		{
			Func<Task> func = async () => await _decorated.Process(cancellationToken);
			await Decorate(func, logLevel: LogLevel.Trace);
		}
	}
}

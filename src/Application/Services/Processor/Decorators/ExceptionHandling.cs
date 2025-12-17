using BlueBrown.Data.DataManagementPatterns.Application.Decorators;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Processor.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class ProcessorExceptionHandlingDecorator : ExceptionHandlingDecorator, IProcessor
	{
		private readonly IProcessor _decorated;

		public ProcessorExceptionHandlingDecorator(
			IProcessor decorated,
			ILogger<ProcessorExceptionHandlingDecorator> logger)
			: base(logger)
		{
			_decorated = decorated;
		}

		public async Task Process(CancellationToken cancellationToken = default)
		{
			Func<Task> func = async () => await _decorated.Process(cancellationToken);
			await Decorate(func);
		}
	}
}

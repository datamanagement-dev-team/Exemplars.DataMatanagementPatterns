using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Processor.Decorators;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Processor
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class DIRegistration
	{
		internal static void RegisterProcessor(this ContainerBuilder builder)
		{
			builder.RegisterDecorator<ProcessorLogAndMetricsDecorator, IProcessor>();
		}
	}
}

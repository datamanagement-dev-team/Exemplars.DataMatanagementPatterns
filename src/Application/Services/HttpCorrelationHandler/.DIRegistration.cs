using Autofac;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpCorrelationHandler
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class DIRegistration
	{
		internal static void RegisterHttpCorrelationHandler(this ContainerBuilder builder)
		{
			builder
				.RegisterType<HttpCorrelationHandler>()
				.As<IHttpCorrelationHandler>()
				.InstancePerLifetimeScope();
		}
	}
}

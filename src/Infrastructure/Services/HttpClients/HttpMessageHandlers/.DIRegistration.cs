using Autofac;
using Microsoft.Extensions.Http.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.HttpMessageHandlers
{
	internal static class DIRegistration
	{
		internal static void RegisterHttpMessageHandlers(this ContainerBuilder builder)
		{
			builder
				.RegisterType<LoggingHttpMessageHandler>()
				.InstancePerLifetimeScope();

			builder
				.RegisterType<CorrelationHttpMessageHandler>()
				.InstancePerLifetimeScope();
		}
	}
}

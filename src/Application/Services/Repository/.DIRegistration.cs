using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Repository.Decorators;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Repository
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class DIRegistration
	{
		internal static void RegisterRepository(this ContainerBuilder builder)
		{
			builder.RegisterDecorator<RepositoryResilienceDecorator, IRepository>();
			builder.RegisterDecorator<RepositoryLogAndMetricsDecorator, IRepository>();
		}
	}
}

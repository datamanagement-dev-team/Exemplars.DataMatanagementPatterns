using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Repository;
using BlueBrown.Data.DataManagementPatterns.Persistence.Services.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Resilience
{
	internal static class DIRegistration
	{
		internal static void RegisterRepository(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(RepositorySettings.ConfigurationKey).Get<RepositorySettings>()!;

			services.AddSingleton<IRepositorySettings>(settings);
			services.AddSingleton<IValidatable>(settings);
		}

		internal static void RegisterRepository(this ContainerBuilder builder)
		{
			builder
				.RegisterType<Repository>()
				.As<IRepository>()
				.InstancePerLifetimeScope();
		}
	}
}

using Autofac;
using BlueBrown.Data.DataManagementPatterns.Api.Controllers;
using BlueBrown.Data.DataManagementPatterns.Api.HealthChecks;
using BlueBrown.Data.DataManagementPatterns.Api.Middlewares;
using BlueBrown.Data.DataManagementPatterns.Api.OpenApi;

namespace BlueBrown.Data.DataManagementPatterns.Api
{
	internal static class DIRegistration
	{
		internal static void RegisterApiServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddResponseCompression(_options =>
			{
				_options.EnableForHttps = true;
			});

			services.RegisterControllers();

			services.RegisterOpenApi(configuration);

			services.RegisterMiddlewares();

			services.RegisterHealthChecks();
		}
	}

	internal class ApiDIRegistrationModule : Module
	{
		protected override void Load(ContainerBuilder builder) { }
	}
}

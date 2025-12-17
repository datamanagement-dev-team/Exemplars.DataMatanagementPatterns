using BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness;
using Microsoft.Extensions.DependencyInjection;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Readiness
{
	internal static class DIRegistration
	{
		internal static void RegisterReadinessService(this IServiceCollection services)
		{
			services.AddHostedService<ReadinessHostedService>();

			services.AddSingleton<IReadinessService, ReadinessService>();
		}
	}
}

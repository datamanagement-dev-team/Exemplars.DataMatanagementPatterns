using BlueBrown.AspNetCore.HealthChecks.Builder;
using BlueBrown.Common.HealthChecks.Extensions;
using BlueBrown.Common.HealthChecks.Metrics.Extensions;
using BlueBrown.Diagnostics.HealthChecks.DependencyInjection;

namespace BlueBrown.Data.DataManagementPatterns.Api.HealthChecks
{
	internal static class DIRegistration
	{
		internal static void RegisterHealthChecks(this IServiceCollection services)
		{
			services
				.AddHealthChecksCustom()
				.AddCheckReady()
				.UseAppMetricsPublisher();
		}

		internal static void MapHealthChecks(this IEndpointRouteBuilder builder)
		{
			builder.MapHealthChecksCustom();
		}
	}
}

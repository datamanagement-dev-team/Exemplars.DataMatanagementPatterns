using Autofac;
using BlueBrown.Common.HealthChecks.Sql.Configuration;
using BlueBrown.Common.HealthChecks.Sql.Extensions;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Resilience;
using BlueBrown.Diagnostics.HealthChecks.DependencyInjection;
using CommonHealthChecks.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Persistence
{
	public static class DIRegistration
	{
		public static void RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
		{
			var sqlDatabaseSettings = configuration.GetSection(SqlDatabaseSettings.ConfigurationKey).Get<SqlDatabaseSettings>()!;

			var connectionString = string.Format(
				sqlDatabaseSettings.ConnectionStringTemplate,
				sqlDatabaseSettings.DataManagementPatterns.DataSource,
				sqlDatabaseSettings.DataManagementPatterns.InitialCatalog,
				sqlDatabaseSettings.DataManagementPatterns.Username,
				sqlDatabaseSettings.DataManagementPatterns.Password);

			var persistenceSettings = new PersistenceSettings(connectionString);

			var errors = persistenceSettings.Validate();
			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			services.AddSingleton<IPersistenceSettings>(persistenceSettings);

			services
				.AddHealthChecksCustom()
				.AddCheckSql(
					name: nameof(SqlDatabaseSettings.DataManagementPatterns),
					sqlConfiguration: new SqlConfiguration
					{
						ConnectionString = connectionString
					},
					healthCheckPriority: HealthCheckPriority.Critical,
					healthCheckProbe: HealthCheckProbe.Ready,
					tags: new string[] { });

			services.RegisterRepository(configuration);
		}
	}

	public class PersistenceDIRegistrationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterRepository();
		}
	}
}

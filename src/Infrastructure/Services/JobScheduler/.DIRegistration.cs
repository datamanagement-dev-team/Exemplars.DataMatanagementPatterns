using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.JobScheduler;
using BlueBrown.Data.DataManagementPatterns.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quartz;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler
{
	internal static class DIRegistration
	{
		internal static void RegisterJobScheduler(this IServiceCollection services, IConfiguration configuration)
		{
			using var serviceProvider = services.BuildServiceProvider();
			var persistenceSettings = serviceProvider.GetRequiredService<IPersistenceSettings>();
			var errors = persistenceSettings.Validate();

			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			var settings = configuration.GetSection(JobSchedulerSettings.ConfigurationKey).Get<JobSchedulerSettings>()!;
			errors = settings.Validate();

			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			services.AddQuartz(_configurator =>
			{
				//name is unique across cluster based on this link:
				//https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#main-configuration
				_configurator.SchedulerName = "DataManagementPatterns";

				//name is unique for each instance running the same job based on this link:
				//https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#main-configuration
				_configurator.SchedulerId = Environment.MachineName;

				_configurator.UsePersistentStore(_options =>
				{
					_options.UseSqlServer(_options =>
					{
						_options.ConnectionString = persistenceSettings.DataManagementPatternsConnectionString;

						_options.TablePrefix = "QRTZ.";
					});

					_options.UseClustering(_options =>
					{
						_options.CheckinInterval = TimeSpan.FromSeconds(settings.SchedulerClusteringCheckInIntervalInSeconds);
					});

					_options.UseNewtonsoftJsonSerializer();
				});
			});

			services.Configure<QuartzOptions>(_options =>
			{
				_options.MisfireThreshold = TimeSpan.FromSeconds(settings.SchedulerMisfireThresholdInSeconds);
			});

			services.AddQuartzHostedService(_options =>
			{
				_options.WaitForJobsToComplete = true;
			});
		}

		internal static void RegisterJobScheduler(this ContainerBuilder builder)
		{
			builder
				.RegisterType<JobScheduler>()
				.As<IJobScheduler>()
				.SingleInstance();
		}
	}
}

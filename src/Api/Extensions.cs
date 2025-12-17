using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlueBrown.Data.DataManagementPatterns.Api.HealthChecks;
using BlueBrown.Data.DataManagementPatterns.Api.Middlewares;
using BlueBrown.Data.DataManagementPatterns.Api.OpenApi;
using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness;
using BlueBrown.Data.DataManagementPatterns.Infrastructure;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Metrics;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.OpenTelemetry;
using BlueBrown.Data.DataManagementPatterns.Persistence;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Api
{
	internal static class Extensions
	{
		internal static void Configure(this IHostBuilder builder, IConfiguration configuration)
		{
			builder.UseServiceProviderFactory(new AutofacServiceProviderFactory(_builder =>
			{
				_builder.RegisterModule<ApiDIRegistrationModule>();
				_builder.RegisterModule<ApplicationDIRegistrationModule>();
				_builder.RegisterModule<InfrastructureDIRegistrationModule>();
				_builder.RegisterModule<PersistenceDIRegistrationModule>();
			}));

			builder.ConfigureMetrics(configuration);
		}

		internal static void Configure(this IConfigurationBuilder builder, string environmentName)
		{
			builder.Sources.Clear();

			builder.AddEnvironmentVariables();
			builder.ConfigureOpenTelemetrySettings();
			builder.AddJsonFile(path: "appsettings.json", optional: true);
			builder.AddConsul();
			builder.AddJsonFile(path: $"appsettings.{environmentName}.json", optional: true);
		}

		internal static void Configure(this ILoggingBuilder builder)
		{
			builder.ClearProviders();
			builder.SetMinimumLevel(LogLevel.Trace);
			builder.AddNlog();
		}

		internal static void Configure(this IServiceCollection services, IConfiguration configuration)
		{
			services.RegisterApiServices(configuration);
			services.RegisterApplicationServices();
			services.RegisterPersistenceServices(configuration);
			services.RegisterInfrastructureServices(configuration);
		}

		internal static void Configure(this IApplicationBuilder builder)
		{
			builder.UseRouting();
			builder.UseResponseCompression();
			builder.UseMiddleware<ExceptionHandlingMiddleware>();

			builder.UseEndpoints(_builder =>
			{
				_builder.MapControllers();
				_builder.MapHealthChecks();
				_builder.MapGet("/", () => "Services.Data.DataManagementPatterns is up");
			});

			builder.UseMetrics();
			builder.UseOpenApi();
		}

		internal static void Configure(this IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
		{
			var logger = Infrastructure.Extensions.CreateLogger<Program>();

			hostApplicationLifetime.ApplicationStarted.Register(async () =>
			{
				try
				{
					var readinessService = serviceProvider.GetRequiredService<IReadinessService>();
					await readinessService.Update(isReady: true);

					var validatables = serviceProvider
						.GetServices<IValidatable>()
						.ToList();

					var errors = validatables
						.SelectMany(_validatable => _validatable.Validate())
						.ToList();

					if (errors.Count > 0)
					{
						var exceptionMessage = JsonConvert.SerializeObject(errors);

						var exception = new Exception(exceptionMessage);

						throw exception;
					}

					logger.LogInformation("application started");
				}
				catch (Exception exception)
				{
					logger.LogError(exception, string.Empty);
				}
			});

			hostApplicationLifetime.ApplicationStopping.Register(() =>
			{
				try
				{
					logger.LogInformation("application stopping");
				}
				catch (Exception exception)
				{
					logger.LogError(exception, string.Empty);
				}
			});

			hostApplicationLifetime.ApplicationStopped.Register(() =>
			{
				try
				{
					logger.LogInformation("application stopped");
				}
				catch (Exception exception)
				{
					logger.LogError(exception, string.Empty);
				}
			});
		}

		internal static Shared.Response MapToSharedResponse(this Application.Response commandResponse)
		{
			Shared.Response response;
			if (!commandResponse.IsSuccess)
				response = Shared.Response.CreateErrored(commandResponse.StatusCode, commandResponse.ErrorMessage!, (Shared.ErrorCode)commandResponse.ErrorCode!);
			else
				response = Shared.Response.Create(commandResponse.StatusCode);
			return response;
		}
		internal static Shared.Response<TResult> MapToSharedResponse<TResult>(this Application.Response<TResult> commandResponse)
		{
			Shared.Response<TResult> response;
			if (!commandResponse.IsSuccess)
				response = Shared.Response<TResult>.CreateErrored(commandResponse.StatusCode, commandResponse.ErrorMessage!, (Shared.ErrorCode)commandResponse.ErrorCode!);
			else
				response = Shared.Response<TResult>.Create(commandResponse.StatusCode, commandResponse.Result!);
			return response;
		}
	}
}

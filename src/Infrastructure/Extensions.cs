using BlueBrown.Common.Consul.Modules;
using BlueBrown.Common.Consul.NetConfiguration.Extensions;
using BlueBrown.Common.Consul.RegistrationSource;
using BlueBrown.Common.HealthChecks.Consul.Extensions;
using BlueBrown.Common.HealthChecks.Vault.Extensions;
using BlueBrown.Common.Vault.Modules;
using BlueBrown.Common.Vault.Modules.Configurations;
using BlueBrown.Common.Vault.ReadSecret;
using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Diagnostics.HealthChecks.DependencyInjection;
using CommonHealthChecks.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure
{
	public static class Extensions
	{
		public static ILogger<TClass> CreateLogger<TClass>()
			where TClass : class
		{
			var loggerProviders = new List<ILoggerProvider>
			{
				new NLogLoggerProvider()
			};

			var loggerFactory = new LoggerFactory(loggerProviders);

			return loggerFactory.CreateLogger<TClass>();
		}

		public static void ShutdownLogging()
		{
			NLog.LogManager.Shutdown();
		}

		public static void AddNlog(this ILoggingBuilder builder)
		{

			builder.AddNLog("nlog.config");
		}

		internal static void RegisterConsul(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(nameof(Settings)).Get<Settings>()!;

			var vaultModuleConfiguration = new VaultModuleConfiguration
			{
				AbsoluteExpirationMinutes = settings.VaultAbsoluteExpirationInMinutes,
				VaultServerUriWithPort = settings.VaultUrl
			};

			services
				.RegisterConsulModule(
					majorVersion: settings.ConsulMajorVersion,
					refreshConfigurationInSeconds: settings.ConsulRefreshConfigurationInSeconds,
					consulUrl: settings.ConsulUrl,
					consulToken: settings.ConsulToken,
					prefixPath: settings.ConsulPrefixPath)
				.AddSingleton(vaultModuleConfiguration)
				.RegisterVaultModule(
					roleId: settings.VaultRoleId,
					secretId: settings.VaultSecretId)
				.ConfigurationObjectsRegistrationSourceModule()
				.AddHealthChecksCustom()
				.AddCheckConsul(
					name: "Consul",
					healthCheckPriority: HealthCheckPriority.Critical,
					healthCheckProbe: HealthCheckProbe.ReadyStartup,
					tags: new string[] { })
				.AddCheckVault(
					name: "Vault",
					healthCheckPriority: HealthCheckPriority.Critical,
					healthCheckProbe: HealthCheckProbe.ReadyStartup,
					tags: new string[] { });
		}

		public static void AddConsul(this IConfigurationBuilder builder)
		{
			var configuration = builder.Build();

			var settings = configuration.GetSection(nameof(Settings)).Get<Settings>()!;

			var errors = settings.Validate();
			if (errors.Count > 0)
			{
				var exceptionMessage = JsonConvert.SerializeObject(errors);
				var exception = new Exception(exceptionMessage);
				throw exception;
			}

			VaultConfigSecretEvaluator.roleId = settings.VaultRoleId;
			VaultConfigSecretEvaluator.secretId = settings.VaultSecretId;
			VaultConfigSecretEvaluator.vaultServerUriWithPort = settings.VaultUrl;

			builder.AddConsulForConfiguration(
				mainKey: settings.ConsulMainKey,
				key: settings.ConsulKey,
				options: _source =>
				{
					_source.Optional = true;

					_source.ConsulConfigurationOptions = _configuration =>
					{
						_configuration.Address = new Uri(settings.ConsulUrl);

						_configuration.Token = settings.ConsulToken;
					};
				},
				VaultConfigSecretEvaluator.EvaluateSecretsWithVault);
		}

		//todo check if some of those values are hard coded instead of read from configuration
		private record Settings : ISettings
		{
			public string ConsulMainKey { get; init; } = string.Empty;
			public string ConsulKey { get; init; } = string.Empty;
			public string ConsulUrl { get; init; } = string.Empty;
			public string ConsulToken { get; init; } = string.Empty;
			public int ConsulMajorVersion { get; init; }
			public int ConsulRefreshConfigurationInSeconds { get; init; }
			public string ConsulPrefixPath { get; init; } = string.Empty;
			public string VaultRoleId { get; init; } = string.Empty;
			public string VaultSecretId { get; init; } = string.Empty;
			public string VaultUrl { get; init; } = string.Empty;
			public int VaultAbsoluteExpirationInMinutes { get; init; }

			public IReadOnlyCollection<string> Validate()
			{
				var errors = new List<string>();

				if (string.IsNullOrWhiteSpace(ConsulMainKey))
					errors.Add($"{nameof(ConsulMainKey)} should not be null");

				if (string.IsNullOrWhiteSpace(ConsulKey))
					errors.Add($"{nameof(ConsulKey)} should not be null");

				if (string.IsNullOrWhiteSpace(ConsulUrl))
					errors.Add($"{nameof(ConsulUrl)} should not be null");

				if (string.IsNullOrWhiteSpace(ConsulToken))
					errors.Add($"{nameof(ConsulToken)} should not be null");

				if (ConsulMajorVersion < 1)
					errors.Add($"{nameof(ConsulMajorVersion)} should be greater than 0");

				if (ConsulRefreshConfigurationInSeconds < 1)
					errors.Add($"{nameof(ConsulRefreshConfigurationInSeconds)} should be greater than 0");

				if (string.IsNullOrWhiteSpace(ConsulPrefixPath))
					errors.Add($"{nameof(ConsulPrefixPath)} should not be null");

				if (string.IsNullOrWhiteSpace(VaultRoleId))
					errors.Add($"{nameof(VaultRoleId)} should not be null");

				if (string.IsNullOrWhiteSpace(VaultSecretId))
					errors.Add($"{nameof(VaultSecretId)} should not be null");

				if (string.IsNullOrWhiteSpace(VaultUrl))
					errors.Add($"{nameof(VaultUrl)} should not be null");

				if (VaultAbsoluteExpirationInMinutes < 1)
					errors.Add($"{nameof(VaultAbsoluteExpirationInMinutes)} should be greater than 0");

				return errors;
			}
		}
	}
}

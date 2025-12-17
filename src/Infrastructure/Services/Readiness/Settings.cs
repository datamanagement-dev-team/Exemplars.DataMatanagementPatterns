using BlueBrown.Common.Consul.Configurations;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Readiness
{
	internal record ReadinessServiceSettings : IConfigurationItem
	{
		private readonly IReadinessService _readinessService;
		private readonly ILogger<ReadinessServiceSettings> _logger;

		public bool IsReady { get; init; } = true;

		public ReadinessServiceSettings(
			IReadinessService readinessService,
			ILogger<ReadinessServiceSettings> logger)
		{
			_readinessService = readinessService;
			_logger = logger;
		}

		public void UpdateOnConfigWasDone()
		{
			_logger.LogInformation(
				"{0} was updated to {1}",
				nameof(ReadinessServiceSettings),
				JsonConvert.SerializeObject(this));

			_readinessService.Update(IsReady).GetAwaiter().GetResult();
		}
	}
}

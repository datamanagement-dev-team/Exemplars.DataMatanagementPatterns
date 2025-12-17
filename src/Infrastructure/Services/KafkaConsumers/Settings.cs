using BlueBrown.ActivityPublishers.Shared.Consumers.Settings;
using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers
{
	internal record KafkaInfrastructureSettings : IKafkaInfrastructureSettings, ISettings
	{
		public const string ConfigurationKey = "kafkainfrastructureconfiguration";

		public string BootstrapServers { get; init; } = string.Empty;
		public string ClientId { get; init; } = string.Empty;
		public string GroupId { get; init; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(BootstrapServers))
				errors.Add($"{nameof(IKafkaInfrastructureSettings.BootstrapServers)} should not be null");

			if (string.IsNullOrWhiteSpace(ClientId))
				errors.Add($"{nameof(IKafkaInfrastructureSettings.ClientId)} should not be null");

			if (string.IsNullOrWhiteSpace(GroupId))
				errors.Add($"{nameof(IKafkaInfrastructureSettings.GroupId)} should not be null");

			return errors;
		}
	}
}

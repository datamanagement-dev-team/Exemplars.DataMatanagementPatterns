using BlueBrown.ActivityPublishers.Shared.Consumers.Settings;
using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers
{
	internal record KafkaInfrastructureSettings : IKafkaInfrastructureSettings, ISettings
	{
		private string _clientId = string.Empty;

		public const string ConfigurationKey = "kafkainfrastructureconfiguration";

		public string BootstrapServers { get; init; } = string.Empty;
		public string ClientId
		{
			get
			{
				return _clientId;
			}
			init
			{
				_clientId = $"{value}_{Environment.MachineName}";
			}
		}
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

using BlueBrown.ActivityPublishers.Shared.Consumers.CasinoRounds;
using BlueBrown.ActivityPublishers.Shared.Consumers.Settings;
using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers.CasinoRound
{
	internal record CasinoRoundKafkaSettings : ICasinoRoundKafkaSettings, IValidatable
	{
		public const string ConfigurationKey = "casinoroundkafkaconfiguration";

		public bool EnableLogs { get; init; }
		public int BatchSize { get; init; }
		public PartitionPositionOffset PartitionPositionOffset { get; init; }
		public string TopicName { get; init; } = string.Empty;
		public string? OptionalGroupId { get; init; }
		public bool Enable { get; init; }

		public IReadOnlyCollection<string> Validate()
		{
			throw new NotImplementedException();
		}
	}
}

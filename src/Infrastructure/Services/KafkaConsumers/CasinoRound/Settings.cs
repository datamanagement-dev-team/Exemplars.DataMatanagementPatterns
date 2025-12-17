using BlueBrown.ActivityPublishers.Shared.Consumers.CasinoRounds;
using BlueBrown.ActivityPublishers.Shared.Consumers.Settings;
using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers.CasinoRound
{
	internal record CasinoRoundKafkaSettings : ICasinoRoundKafkaSettings, ISettings
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
			var errors = new List<string>();

			if (BatchSize < 1)
				errors.Add($"{nameof(ICasinoRoundKafkaSettings.BatchSize)} should be greater than zero");

			if (string.IsNullOrWhiteSpace(TopicName))
				errors.Add($"{nameof(ICasinoRoundKafkaSettings.TopicName)} should not be null");

			if (string.IsNullOrWhiteSpace(OptionalGroupId))
				errors.Add($"{nameof(ICasinoRoundKafkaSettings.OptionalGroupId)} should not be null or empty");

			return errors;
		}
	}
}

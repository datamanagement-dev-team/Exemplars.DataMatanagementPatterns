using BlueBrown.ActivityPublishers.Shared.Consumers.CasinoRounds;
using BlueBrown.ActivityPublishers.Shared.Consumers.Interfaces;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers.CasinoRound
{
	internal class CasinoRoundKafkaConsumer : ICasinoRoundKafkaConsumer
	{
		public Task Consume(
			IReadOnlyCollection<ActivityPublishers.Shared.Activities.CasinoRounds.CasinoRound> kafkaEntities, 
			IReadOnlyCollection<ITopicPartitionOffset>? topicPartitionOffsets, 
			CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}

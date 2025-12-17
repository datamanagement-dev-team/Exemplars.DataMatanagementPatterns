using BlueBrown.ActivityPublishers.Shared.Consumers.Interfaces;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers
{
	internal class TopicPartitionOffsetRepository : ITopicPartitionOffsetRepository
	{
		public async Task<IReadOnlyCollection<ITopicPartitionOffset>> GetPartitionOffsets(CancellationToken cancellationToken)
		{
			return new List<ITopicPartitionOffset>();
		}
	}
}

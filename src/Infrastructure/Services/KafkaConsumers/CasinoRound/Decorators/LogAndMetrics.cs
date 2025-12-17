using BlueBrown.ActivityPublishers.Shared.Consumers.CasinoRounds;
using BlueBrown.ActivityPublishers.Shared.Consumers.Interfaces;
using BlueBrown.Data.DataManagementPatterns.Application.Decorators;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.KafkaConsumers.CasinoRound.Decorators
{
	internal class CasinoRoundKafkaConsumerLogAndMetricsDecorator : LogAndMetricsDecorator, ICasinoRoundKafkaConsumer
	{
		private readonly ICasinoRoundKafkaConsumer _decorated;
		private readonly IMetrics _metrics;

		public CasinoRoundKafkaConsumerLogAndMetricsDecorator(
			ICasinoRoundKafkaConsumer decorated,
			IMetrics metrics,
			ILogger<CasinoRoundKafkaConsumerLogAndMetricsDecorator> logger)
			: base(
				  contextName: "infrastructure",
				  serviceName: nameof(ICasinoRoundKafkaConsumer),
				  metrics: metrics,
				  logger: logger)
		{
			_decorated = decorated;
			_metrics = metrics;
		}

		public async Task Consume(
			IReadOnlyCollection<ActivityPublishers.Shared.Activities.CasinoRounds.CasinoRound> kafkaEntities, 
			IReadOnlyCollection<ITopicPartitionOffset>? topicPartitionOffsets, 
			CancellationToken cancellationToken)
		{
			Func<Task> func = async () => await _decorated.Consume(kafkaEntities, topicPartitionOffsets, cancellationToken);

			var minCreatedAt = kafkaEntities.Min(x => x.Created);

			var replicationDelayInMilliseconds = (int)(DateTimeOffset.UtcNow - minCreatedAt.ToUniversalTime()).TotalMilliseconds;

			await Decorate(func, kafkaEntities.Count, logLevel: LogLevel.Trace);

			var extraTags = new Dictionary<string, string>
			{
				{ "entity", typeof(ActivityPublishers.Shared.Activities.CasinoRounds.CasinoRound).Name }
			};
			var tags = GetTags(extraTags);
			tags.Add("metric", "replication_delay");
			_metrics.MeasureGauge(replicationDelayInMilliseconds, tags);
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Decorators;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.MessageBroker.Filters
{
	internal class LogAndMetricsFilter<TEntity> : LogAndMetricsDecorator, IFilter<ConsumeContext<Batch<TEntity>>>
		where TEntity : class
	{
		private readonly IMetrics _metrics;
		private readonly ILogger<LogAndMetricsDecorator> _logger;

		public LogAndMetricsFilter(
			IMetrics metrics,
			ILogger<LogAndMetricsDecorator> logger)
			: base(
				  contextName: "infrastructure",
				  serviceName: nameof(LogAndMetricsFilter<TEntity>),
				  metrics: metrics,
				  logger: logger)
		{
			_metrics = metrics;
			_logger = logger;
		}

		public void Probe(ProbeContext context)
		{
			context.CreateFilterScope("LogAndMetricsFilter");
		}

		public async Task Send(ConsumeContext<Batch<TEntity>> context, IPipe<ConsumeContext<Batch<TEntity>>> next)
		{
			using var loggerScope = _logger.BeginScope("entity", typeof(TEntity).Name);

			Func<Task> func = async () => await Send(context, next);
			var extraTags = new Dictionary<string, string>
			{
				{ "entity", typeof(TEntity).Name }
			};
			await Decorate(func: func, count: context.Message.Length, extraTags: extraTags, method: nameof(IConsumer<>.Consume), logLevel: LogLevel.Trace);

			var minCreatedAt = context.Message.Min(_consumeContext => _consumeContext.SentTime!.Value);
			var replicationDelayInMilliseconds = (int)(DateTimeOffset.UtcNow - minCreatedAt.ToUniversalTime()).TotalMilliseconds;

			var tags = GetTags(extraTags: extraTags, method: nameof(IConsumer<>.Consume));
			tags.Add("metric", "replication_delay");
			_metrics.MeasureGauge(replicationDelayInMilliseconds, tags);
		}
	}
}

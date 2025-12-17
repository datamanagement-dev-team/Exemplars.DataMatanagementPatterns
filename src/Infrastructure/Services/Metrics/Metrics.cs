using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Timer;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Metrics
{
	internal class Metrics : Application.Services.Metrics.IMetrics
	{
		private readonly IMetrics _metrics;

		public Metrics(IMetrics metrics)
		{
			_metrics = metrics;
		}

		public void MeasureTime(long value, IReadOnlyDictionary<string, string> tags)
		{
			var metricTags = new MetricTags(tags.Keys.ToArray(), tags.Values.ToArray());

			var options = new TimerOptions
			{
				DurationUnit = TimeUnit.Milliseconds,
				Name = "timer",
				Tags = metricTags
			};

			_metrics.Measure.Timer.Time(options, value);
		}

		public void MeasureGauge(int value, IReadOnlyDictionary<string, string> tags)
		{
			var metricTags = new MetricTags(tags.Keys.ToArray(), tags.Values.ToArray());

			var options = new GaugeOptions
			{
				MeasurementUnit = Unit.Items,
				Name = "gauge",
				Tags = metricTags
			};

			_metrics.Measure.Gauge.SetValue(options, value);
		}

		public void IncreaseCounter(int amount, IReadOnlyDictionary<string, string> tags)
		{
			var metricTags = new MetricTags(tags.Keys.ToArray(), tags.Values.ToArray());

			var options = new CounterOptions
			{
				Name = "counter",
				Tags = metricTags,
				MeasurementUnit = Unit.Items
			};

			_metrics.Measure.Counter.Increment(options, amount);
		}
	}
}

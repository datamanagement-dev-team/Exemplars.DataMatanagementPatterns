using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BlueBrown.Data.DataManagementPatterns.Application.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public abstract class LogAndMetricsDecorator
	{
		private readonly string _contextName;
		private readonly string _serviceName;
		private readonly IMetrics _metrics;
		private readonly ILogger<LogAndMetricsDecorator> _logger;

		protected LogAndMetricsDecorator(
			string contextName,
			string serviceName,
			IMetrics metrics,
			ILogger<LogAndMetricsDecorator> logger)
		{
			_contextName = contextName;
			_serviceName = serviceName;
			_metrics = metrics;
			_logger = logger;
		}

		protected async Task Decorate(Func<Task> func, int? count = default, IReadOnlyDictionary<string, string>? extraTags = default, [CallerMemberName] string method = "", LogLevel logLevel = LogLevel.Information)
		{
			_logger.Log(
				logLevel,
				"[{0}] started",
				method);

			var stopwatch = Stopwatch.StartNew();

			await func();

			stopwatch.Stop();

			using var loggerScope = _logger.BeginScope(LoggingConstants.Duration, stopwatch.ElapsedMilliseconds.ToString());

			_logger.LogInformation("[{0}] completed", method);

			var tags = new Dictionary<string, string>
			{
				{ "context", _contextName },
				{ LoggingConstants.Service, _serviceName },
				{ LoggingConstants.Method, method }
			};

			if (extraTags is not null)
			{
				foreach (var extraTag in extraTags)
					tags.Add(extraTag.Key, extraTag.Value);
			}

			_metrics.MeasureTime(stopwatch.ElapsedMilliseconds, tags);

			if (count is not null)
				_metrics.MeasureGauge(count.Value, tags);
		}

		protected async Task<T> Decorate<T>(Func<Task<T>> func, int? count = default, IReadOnlyDictionary<string, string>? extraTags = default, [CallerMemberName] string method = "", LogLevel logLevel = LogLevel.Information)
		{
			_logger.Log(
				logLevel,
				"[{0}] started",
				method);

			var stopwatch = Stopwatch.StartNew();

			var result = await func();

			stopwatch.Stop();

			using var loggerScope = _logger.BeginScope(LoggingConstants.Duration, stopwatch.ElapsedMilliseconds.ToString());

			_logger.LogInformation("[{0}] completed", method);

			var tags = new Dictionary<string, string>
			{
				{ "context", _contextName },
				{ LoggingConstants.Service, _serviceName },
				{ LoggingConstants.Method, method }
			};

			if (extraTags is not null)
			{
				foreach (var extraTag in extraTags)
					tags.Add(extraTag.Key, extraTag.Value);
			}

			_metrics.MeasureTime(stopwatch.ElapsedMilliseconds, tags);

			if (count is not null)
				_metrics.MeasureGauge(count.Value, tags);

			return result;
		}

		protected void Decorate(Action func, int? count = default, IReadOnlyDictionary<string, string>? extraTags = default, [CallerMemberName] string method = "", LogLevel logLevel = LogLevel.Information)
		{
			_logger.Log(
				logLevel,
				"[{0}] started",
				method);

			var stopwatch = Stopwatch.StartNew();

			func();

			stopwatch.Stop();

			using var loggerScope = _logger.BeginScope(LoggingConstants.Duration, stopwatch.ElapsedMilliseconds.ToString());

			_logger.LogInformation("[{0}] completed", method);

			var tags = new Dictionary<string, string>
			{
				{ "context", _contextName },
				{ LoggingConstants.Service, _serviceName },
				{ LoggingConstants.Method, method }
			};

			if (extraTags is not null)
			{
				foreach (var extraTag in extraTags)
					tags.Add(extraTag.Key, extraTag.Value);
			}

			_metrics.MeasureTime(stopwatch.ElapsedMilliseconds, tags);

			if (count is not null)
				_metrics.MeasureGauge(count.Value, tags);
		}

		protected T Decorate<T>(Func<T> func, int? count = default, IReadOnlyDictionary<string, string>? extraTags = default, [CallerMemberName] string method = "", LogLevel logLevel = LogLevel.Information)
		{
			_logger.Log(
				logLevel,
				"[{0}] started",
				method);

			var stopwatch = Stopwatch.StartNew();

			var result = func();

			stopwatch.Stop();

			using var loggerScope = _logger.BeginScope(LoggingConstants.Duration, stopwatch.ElapsedMilliseconds.ToString());

			_logger.LogInformation("[{0}] completed", method);

			var tags = new Dictionary<string, string>
			{
				{ "context", _contextName },
				{ LoggingConstants.Service, _serviceName },
				{ LoggingConstants.Method, method }
			};

			if (extraTags is not null)
			{
				foreach (var extraTag in extraTags)
					tags.Add(extraTag.Key, extraTag.Value);
			}

			_metrics.MeasureTime(stopwatch.ElapsedMilliseconds, tags);

			if (count is not null)
				_metrics.MeasureGauge(count.Value, tags);

			return result;
		}

		protected Dictionary<string, string> GetTags(IReadOnlyDictionary<string, string>? extraTags = default, [CallerMemberName] string method = "")
		{
			var tags = new Dictionary<string, string>
			{
				{ "context", _contextName },
				{ "service", _serviceName },
				{ "method", method }
			};

			if (extraTags is not null && extraTags.Count > 0)
			{
				foreach (var tag in extraTags)
					tags.Add(tag.Key, tag.Value);
			}

			return tags;
		}
	}
}

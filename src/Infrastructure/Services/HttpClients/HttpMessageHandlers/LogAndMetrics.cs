using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.HttpMessageHandlers
{
	internal class LogAndMetricsHttpMessageHandler : DelegatingHandler
	{
		private readonly ILogger<LogAndMetricsHttpMessageHandler> _logger;
		private readonly IMetrics _metrics;

		public LogAndMetricsHttpMessageHandler(
			ILogger<LogAndMetricsHttpMessageHandler> logger,
			IMetrics metrics)
		{
			_logger = logger;
			_metrics = metrics;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
		{
			request.Headers.TryGetValues(LoggingConstants.Service, out var values);
			var serviceName = values!.FirstOrDefault();
			var service = string.Empty;
			if (serviceName is not null
				&& !string.IsNullOrWhiteSpace(serviceName.ToString()))
				service = serviceName.ToString()!;
			request.Headers.Remove(LoggingConstants.Service);
			using var serviceLoggerScope = _logger.BeginScope(LoggingConstants.Service, service);

			request.Headers.TryGetValues(LoggingConstants.Method, out values);
			var methodName = values!.FirstOrDefault();
			var method = string.Empty;
			if (methodName is not null
				&& !string.IsNullOrWhiteSpace(methodName.ToString()))
				method = methodName.ToString()!;
			request.Headers.Remove(LoggingConstants.Method);
			using var methodLoggerScope = _logger.BeginScope(LoggingConstants.Method, method);

			request.Headers.TryGetValues(LoggingConstants.HttpRequestUrl, out values);
			var httpRequestUrlName = values?.FirstOrDefault();
			var httpRequestUrl = string.Empty;
			if (httpRequestUrlName is not null
				&& !string.IsNullOrWhiteSpace(httpRequestUrlName.ToString()))
				httpRequestUrl = httpRequestUrlName.ToString()!;
			request.Headers.Remove(LoggingConstants.HttpRequestUrl);
			httpRequestUrl = string.IsNullOrWhiteSpace(httpRequestUrl) ? request.RequestUri!.AbsolutePath : httpRequestUrl;
			using var httpRequestUrlLoggerScope = _logger.BeginScope(LoggingConstants.HttpRequestUrl, httpRequestUrl);

			_logger.LogInformation($"{serviceName} [{methodName}] http request started");

			var stopwatch = Stopwatch.StartNew();

			var response = await base.SendAsync(request, cancellationToken);

			stopwatch.Stop();
			var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

			using var httpRequestStatusCodeLoggerScope = _logger.BeginScope(LoggingConstants.HttpRequestStatusCode, response.StatusCode);
			using var httpRequestDurationLoggerScope = _logger.BeginScope(LoggingConstants.Duration, elapsedMilliseconds);

			_logger.LogInformation($"{serviceName} [{methodName}] http request completed");

			var tags = new Dictionary<string, string>
			{
				{ "context", "infrastructure" },
				{ LoggingConstants.Service, service },
				{ LoggingConstants.Method, method },
				{ LoggingConstants.HttpRequestUrl, httpRequestUrl },
				{ LoggingConstants.HttpRequestStatusCode, response.StatusCode.ToString() }
			};
			_metrics.MeasureTime(elapsedMilliseconds, tags);
			_metrics.IncreaseCounter(amount: 1, tags: tags);

			return response;
		}
	}
}

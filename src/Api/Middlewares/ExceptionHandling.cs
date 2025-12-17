using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpCorrelationHandler;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using BlueBrown.Data.DataManagementPatterns.Shared;
using System.Diagnostics;
using System.Net;

namespace BlueBrown.Data.DataManagementPatterns.Api.Middlewares
{
	internal class ExceptionHandlingMiddleware : IMiddleware
	{
		private readonly IHttpCorrelationHandler _httpCorrelationHandler;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;
		private readonly IMetrics _metrics;

		public ExceptionHandlingMiddleware(
			IHttpCorrelationHandler httpCorrelationHandler,
			ILogger<ExceptionHandlingMiddleware> logger,
			IMetrics metrics)
		{
			_httpCorrelationHandler = httpCorrelationHandler;
			_logger = logger;
			_metrics = metrics;
		}

		public async Task InvokeAsync(HttpContext httpContext, RequestDelegate nextMiddleware)
		{
			var correlationId = GetCorrelationId(httpContext.Request.Headers);
			_httpCorrelationHandler.SetCorrelationId(correlationId);
			_httpCorrelationHandler.SetTraceId(Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier);

			using var correlationIdloggerScope = _logger.BeginScope(LoggingConstants.CorrelationId, _httpCorrelationHandler.CorrelationId ?? string.Empty);
			using var traceIdloggerScope = _logger.BeginScope(LoggingConstants.TraceId, _httpCorrelationHandler.TraceId ?? string.Empty);

			try
			{
				httpContext.Response.OnStarting(() =>
				{
					httpContext.Response.Headers.Append(LoggingConstants.CorrelationId, _httpCorrelationHandler.CorrelationId);
					httpContext.Response.Headers.Append(LoggingConstants.TraceId, _httpCorrelationHandler.TraceId);

					return Task.CompletedTask;
				});

				await nextMiddleware(httpContext);
			}
			catch (Exception exception)
			{
				var tags = GetTags(HttpStatusCode.InternalServerError);
				_metrics.IncreaseCounter(amount: 1, tags: tags);

				_logger.LogError(exception, string.Empty);

				await httpContext.Response.WriteAsJsonAsync(
					value: Shared.Response.CreateErrored(
						statusCode: StatusCodes.Status500InternalServerError,
						errorMessage: exception.Message,
						errorCode: Shared.ErrorCode.GenericError),
					options: Application.Extensions.JsonSerializerOptions,
					cancellationToken: httpContext.RequestAborted);
			}
		}

		private IReadOnlyDictionary<string, string> GetTags(HttpStatusCode statusCode)
		{
			var tags = new Dictionary<string, string>
			{
				{ "context", "api" },
				{ LoggingConstants.HttpRequestStatusCode, statusCode.ToString() }
			};

			return tags;
		}

		private string GetCorrelationId(IHeaderDictionary headers)
		{
			if (!headers.TryGetValue(LoggingConstants.CorrelationId, out var correlationId))
				correlationId = Guid.NewGuid().ToString();

			_httpCorrelationHandler.SetCorrelationId(correlationId!);

			return correlationId!;
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;

namespace BlueBrown.Data.DataManagementPatterns.Api.Filters
{
	internal class LogAndMetricsFilter : IAsyncActionFilter
	{
		private readonly ILogger<LogAndMetricsFilter> _logger;
		private readonly IMetrics _metrics;

		public LogAndMetricsFilter(
			ILogger<LogAndMetricsFilter> logger,
			IMetrics metrics)
		{
			_logger = logger;
			_metrics = metrics;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var endpointDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
			var controllerName = endpointDescriptor?.ControllerName?.ToLower();
			var actionName = endpointDescriptor?.ActionName?.ToLower();

			var loggerScopeState = GetLoggerScope(controllerName, actionName);

			using var httpRequestLoggerScope = _logger.BeginScope(loggerScopeState);

			var pathInfo = string.Concat(context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);

			_logger.LogInformation("Incoming request started: {0}", pathInfo);

			var stopwatch = Stopwatch.StartNew();

			await next();

			stopwatch.Stop();
			var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			var httpStatusCode = (HttpStatusCode)context.HttpContext.Response.StatusCode;

			using var httpRequestDurationLoggerScope = _logger.BeginScope(LoggingConstants.Duration, elapsedMilliseconds);
			using var httpRequestStatusCodeLoggerScope = _logger.BeginScope(LoggingConstants.HttpRequestStatusCode, httpStatusCode.ToString());

			_logger.LogInformation("Incoming request completed: {0}", pathInfo);
			var tags = GetTags(controllerName, actionName, httpStatusCode);

			_metrics.MeasureTime(elapsedMilliseconds, tags);
			_metrics.IncreaseCounter(amount: 1, tags: tags);
		}

		private Dictionary<string, object> GetLoggerScope(string? controllerName, string? actionName)
		{
			var loggerScopeState = new Dictionary<string, object> { };

			if (!string.IsNullOrWhiteSpace(controllerName))
			{
				loggerScopeState.Add(LoggingConstants.Service, controllerName);
			}

			if (!string.IsNullOrWhiteSpace(actionName))
			{
				loggerScopeState.Add(LoggingConstants.Method, actionName);
			}

			return loggerScopeState;
		}

		private IReadOnlyDictionary<string, string> GetTags(string? controllerName, string? actionName, HttpStatusCode statusCode)
		{
			var tags = new Dictionary<string, string>
			{
				{ "context", "api" },
				{ LoggingConstants.HttpRequestStatusCode, statusCode.ToString() }
			};

			if (!string.IsNullOrWhiteSpace(controllerName))
			{
				tags.Add(LoggingConstants.Service, controllerName);
			}

			if (!string.IsNullOrWhiteSpace(actionName))
			{
				tags.Add(LoggingConstants.Method, actionName);
			}

			return tags;
		}
	}
}
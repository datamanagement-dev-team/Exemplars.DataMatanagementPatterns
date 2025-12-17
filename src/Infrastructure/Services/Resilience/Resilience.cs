using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly.Timeout;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Resilience
{
	internal class ResilienceService : IResilienceService
	{
		private readonly ILogger<ResilienceService> _logger;
		private readonly Polly.ResiliencePipelineBuilder _resiliencePipelineBuilder;
		private readonly string _correlationId;

		public ResilienceService(ILogger<ResilienceService> logger)
		{
			_logger = logger;
			_resiliencePipelineBuilder = new Polly.ResiliencePipelineBuilder();
			_correlationId = Guid.NewGuid().ToString();
		}

		public void AddRetry(RetrySettings settings, Func<RetryPredicateArguments<object>, ValueTask<bool>>? shouldHandle = default)
		{
			var options = new RetryStrategyOptions
			{
				BackoffType = MapToPollyDelayBackoffType(settings.DelayBackoffType),
				Delay = TimeSpan.FromMilliseconds(settings.DelayInMilliseconds),
				MaxRetryAttempts = settings.MaxRetryAttempts,
				ShouldHandle = shouldHandle ??= _args =>
				{
					return ValueTask.FromResult(_args.Outcome.Exception is not null);
				},
				OnRetry = _args =>
				{
					var attemptNumber = _args.AttemptNumber + 1;

					var exception = _args.Outcome.Exception;

					_logger.LogError(
						exception,
						"retry triggered with attempt number: [{0}], correlation id: [{1}]",
						attemptNumber,
						_correlationId);

					return ValueTask.CompletedTask;
				}
			};

			Polly.RetryResiliencePipelineBuilderExtensions.AddRetry(_resiliencePipelineBuilder, options);
		}

		public void AddTimeout(TimeoutSettings settings)
		{
			var options = new TimeoutStrategyOptions
			{
				OnTimeout = _args =>
				{
					_logger.LogError(
						"timeout triggered after: [{0}]ms, correlation id: [{1}]",
						_args.Timeout.TotalMilliseconds,
						_correlationId);
					return ValueTask.CompletedTask;
				},
				Timeout = TimeSpan.FromMilliseconds(settings.TimeoutInMilliseconds)
			};

			Polly.TimeoutResiliencePipelineBuilderExtensions.AddTimeout(
				_resiliencePipelineBuilder,
				options);
		}

		public async Task ExecuteAsync(Func<CancellationToken, ValueTask> operation, CancellationToken cancellationToken = default)
		{
			await _resiliencePipelineBuilder
				.Build()
				.ExecuteAsync(operation, cancellationToken);
		}

		public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> operation, CancellationToken cancellationToken = default)
		{
			return await _resiliencePipelineBuilder
				.Build()
				.ExecuteAsync(operation, cancellationToken);
		}

		private Polly.DelayBackoffType MapToPollyDelayBackoffType(DelayBackoffType delayBackoffType)
		{
			return delayBackoffType switch
			{
				DelayBackoffType.Constant => Polly.DelayBackoffType.Constant,
				DelayBackoffType.Linear => Polly.DelayBackoffType.Linear,
				DelayBackoffType.Exponential => Polly.DelayBackoffType.Exponential,
				_ => Polly.DelayBackoffType.Constant,
			};
		}
	}
}

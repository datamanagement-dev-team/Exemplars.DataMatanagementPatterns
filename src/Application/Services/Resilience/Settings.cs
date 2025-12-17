namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public record ResilienceSettings
	{
		public RetrySettings? Retry { get; init; }
		public TimeoutSettings? Timeout { get; init; }
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public record RetrySettings
	{
		public int MaxRetryAttempts { get; init; }
		public int DelayInMilliseconds { get; init; }
		public DelayBackoffType DelayBackoffType { get; init; }
	}

	public enum DelayBackoffType
	{
		Constant,
		Linear,
		Exponential
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public record TimeoutSettings
	{
		public int TimeoutInMilliseconds { get; init; }
	}
}

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience
{
	public interface IResilienceService
	{
		Task ExecuteAsync(Func<CancellationToken, ValueTask> operation, CancellationToken cancellationToken = default);
		Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> operation, CancellationToken cancellationToken = default);
	}
}

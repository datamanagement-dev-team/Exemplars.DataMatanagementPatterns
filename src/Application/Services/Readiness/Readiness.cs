namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness
{
	public interface IReadinessService
	{
		Task<bool> IsReady(CancellationToken cancellationToken = default);
		Task Update(bool isReady, CancellationToken cancellationToken = default);
	}
}

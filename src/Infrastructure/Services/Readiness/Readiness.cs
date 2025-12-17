using BlueBrown.Data.DataManagementPatterns.Application.Services.Readiness;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Readiness
{
	internal class ReadinessService : IReadinessService
	{
		private readonly Common.HealthChecks.HealthChecks.IReadinessService _readinessService;

		public ReadinessService(Common.HealthChecks.HealthChecks.IReadinessService readinessService)
		{
			_readinessService = readinessService;
		}

		public Task<bool> IsReady(CancellationToken cancellationToken = default)
		{
			return Task.FromResult(_readinessService.GetReady());
		}

		public async Task Update(bool isReady, CancellationToken cancellationToken = default)
		{
			if (isReady)
				await _readinessService.Ready();
			else
				await _readinessService.NotReady();
		}
	}
}

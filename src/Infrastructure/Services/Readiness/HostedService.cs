using Microsoft.Extensions.Hosting;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Readiness
{
	/// <summary>
	/// This is a placeholder hosted service so that the <code><see cref="IReadinessService"/></code> can be resolved.
	/// </summary>
	internal class ReadinessHostedService : IHostedService
	{
		public ReadinessHostedService(ReadinessServiceSettings _) { }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}

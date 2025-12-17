namespace BlueBrown.Data.DataManagementPatterns.Application.Services.JobScheduler
{
	public interface IJobScheduler
	{
		Task InterruptJob(string jobTypeName, CancellationToken cancellationToken = default);
		Task ScheduleJobs(CancellationToken cancellationToken = default);
	}
}

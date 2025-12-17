using System.Diagnostics.CodeAnalysis;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.JobScheduler
{
	public interface IJobSchedulerSettings : ISettings
	{
		int SchedulerClusteringCheckInIntervalInSeconds { get; }
		int SchedulerMisfireThresholdInSeconds { get; }

		JobSettings Job { get; }
	}

	[ExcludeFromCodeCoverage]
	public record JobSettings
	{
		public string? SchedulerTriggerCronExpression { get; init; }
		public int? SchedulerTriggerIntervalInSeconds { get; init; }
	}
}

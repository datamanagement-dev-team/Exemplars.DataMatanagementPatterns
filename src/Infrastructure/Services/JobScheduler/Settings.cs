using BlueBrown.Data.DataManagementPatterns.Application.Services.JobScheduler;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler
{
	internal record JobSchedulerSettings : IJobSchedulerSettings
	{
		public const string ConfigurationKey = "jobschedulerconfiguration";

		public int SchedulerClusteringCheckInIntervalInSeconds { get; init; }
		public int SchedulerMisfireThresholdInSeconds { get; init; }

		public JobSettings Job { get; init; } = new JobSettings();

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (SchedulerClusteringCheckInIntervalInSeconds < 1)
				errors.Add($"{nameof(IJobSchedulerSettings.SchedulerClusteringCheckInIntervalInSeconds)} should be greater than zero");

			if (SchedulerMisfireThresholdInSeconds < 1)
				errors.Add($"{nameof(IJobSchedulerSettings.SchedulerMisfireThresholdInSeconds)} should be greater than zero");

			if (Job == null)
				errors.Add($"{nameof(IJobSchedulerSettings.Job)} should not be null");

			if (Job?.SchedulerTriggerIntervalInSeconds < 1)
				errors.Add($"{nameof(IJobSchedulerSettings.Job.SchedulerTriggerIntervalInSeconds)} should be greater than zero");

			return errors;
		}
	}
}

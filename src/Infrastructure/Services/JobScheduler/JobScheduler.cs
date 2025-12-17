using BlueBrown.Data.DataManagementPatterns.Application.Services.JobScheduler;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler.Jobs;
using Quartz;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler
{
	internal class JobScheduler : IJobScheduler
	{
		private readonly ISchedulerFactory _schedulerFactory;
		private readonly IJobSchedulerSettings _settings;

		public JobScheduler(
			ISchedulerFactory schedulerFactory,
			IJobSchedulerSettings settings)
		{
			_schedulerFactory = schedulerFactory;
			_settings = settings;
		}

		public async Task ScheduleJobs(CancellationToken cancellationToken = default)
		{
			await ScheduleJob<Job>(_settings.Job.SchedulerTriggerIntervalInSeconds!.Value, cancellationToken);
		}

		public async Task InterruptJob(string jobTypeName, CancellationToken cancellationToken = default)
		{
			var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

			var jobKey = new JobKey(jobTypeName, "DataManagementPatterns");
			var jobAlreadyExists = await scheduler.CheckExists(jobKey, cancellationToken);

			if (!jobAlreadyExists)
				return;

			await scheduler.Interrupt(jobKey, cancellationToken);
		}

		private async Task ScheduleJob<TJob>(int intervalInSeconds, CancellationToken cancellationToken = default)
			where TJob : IJob
		{
			var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

			var jobKey = new JobKey(typeof(TJob).Name, "DataManagementPatterns");

			var jobAlreadyExists = await scheduler.CheckExists(jobKey, cancellationToken);

			if (jobAlreadyExists)
				return;

			var triggerKey = new TriggerKey(typeof(TJob).Name, "DataManagementPatterns");

			var triggerAlreadyExists = await scheduler.CheckExists(triggerKey, cancellationToken);

			if (triggerAlreadyExists)
				return;

			var jobBuilder = JobBuilder
				.Create<TJob>()
				.WithIdentity(jobKey);

			var trigggerBuilder = TriggerBuilder
				.Create()
				.WithIdentity(triggerKey)
				.WithSimpleSchedule(_builder =>
				{
					//this was chosen due to this article:
					//https://stackoverflow.com/questions/30358718/getting-quartz-net-to-ignore-misfires
					_builder.WithMisfireHandlingInstructionNextWithRemainingCount();

					_builder
						.WithIntervalInSeconds(intervalInSeconds)
						.RepeatForever();
				});

			var jobDetail = jobBuilder.Build();

			var trigger = trigggerBuilder.Build();

			await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Application.Services.Processor;
using Quartz;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.JobScheduler.Jobs
{
	[DisallowConcurrentExecution]
	internal class Job : IJob
	{
		private readonly IProcessor _processor;

		public Job(IProcessor processor)
		{
			_processor = processor;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			await _processor.Process(context.CancellationToken);
		}
	}
}

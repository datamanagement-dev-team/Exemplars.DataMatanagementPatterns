namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Processor
{
	public interface IProcessor
	{
		Task Process(CancellationToken cancellationToken = default);
	}

	internal class Processor : IProcessor
	{
		public Task Process(CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}
	}
}

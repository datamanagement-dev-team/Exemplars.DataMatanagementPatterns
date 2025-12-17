using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Application.Decorators
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public abstract class ExceptionHandlingDecorator
	{
		private readonly ILogger<ExceptionHandlingDecorator> _logger;

		protected ExceptionHandlingDecorator(ILogger<ExceptionHandlingDecorator> logger)
		{
			_logger = logger;
		}

		public async Task Decorate(Func<Task> func, bool throwException = true)
		{
			try
			{
				await func();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, string.Empty);

				if (throwException)
					throw;
			}
		}

		public async Task<T?> Decorate<T>(Func<Task<T>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, string.Empty);
				return default;
			}
		}

		protected void Decorate(Action func, bool throwException = true)
		{
			try
			{
				func();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, string.Empty);

				if (throwException)
					throw;
			}
		}
	}
}

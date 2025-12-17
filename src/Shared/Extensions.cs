using Microsoft.Extensions.Logging;

namespace BlueBrown.Data.DataManagementPatterns.Shared
{
	internal static class Extensions
	{
		internal static IDisposable BeginScope(this ILogger logger, string key, object value)
		{
			return logger.BeginScope(new KeyValuePair<string, object>(key, value))!;
		}
	}
}

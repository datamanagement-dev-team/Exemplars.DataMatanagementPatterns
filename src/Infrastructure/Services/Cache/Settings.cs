using BlueBrown.Data.DataManagementPatterns.Application.Services.Cache;
using StackExchange.Redis;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Cache
{
	internal record CacheSettings : ICacheSettings
	{
		public const string ConfigurationKey = "cacheconfiguration";

		public string ConnectionStringTemplate { get; set; } = string.Empty;
		public string ConnectionString { get; private set; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(ConnectionString))
				errors.Add($"{nameof(ICacheSettings.ConnectionString)} should not be null");

			try
			{
				var configurationOptions = ConfigurationOptions.Parse(ConnectionString);

				using var connectionMultiplexer = ConnectionMultiplexer.SentinelConnect(configurationOptions);
			}
			catch
			{
				errors.Add($"{nameof(ICacheSettings.ConnectionString)} should be valid");
			}

			return errors;
		}

		public void SetConnectionString(string connectionString)
		{
			ConnectionString = connectionString;
		}
	}

	internal record RedisConnectionSettings
	{
		public const string ConfigurationKey = "redisconnectionconfiguration";

		public string Password { get; init; } = string.Empty;
		public string ServiceName { get; init; } = string.Empty;
		public string Endpoints { get; init; } = string.Empty;
	}
}

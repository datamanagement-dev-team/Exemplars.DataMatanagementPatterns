using BlueBrown.Data.DataManagementPatterns.Application.Services.Cache;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.Cache
{
	internal class Cache : ICache
	{
		private readonly IDatabase _database;

		public Cache(IConnectionMultiplexer connectionMultiplexer)
		{
			_database = connectionMultiplexer.GetDatabase();
		}

		public async Task<TValue?> Get<TKey, TValue>(TKey key)
		{
			var value = await _database.StringGetAsync(CreateRedisKey(key));

			var valueString = value.ToString();

			if (string.IsNullOrEmpty(valueString))
				return default;

			return JsonConvert.DeserializeObject<TValue>(valueString);
		}

		public async Task Remove<TKey>(TKey key)
		{
			await _database.KeyDeleteAsync(CreateRedisKey(key));
		}

		public async Task Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expiration = null)
		{
			var valueString = JsonConvert.SerializeObject(value);

			await _database.StringSetAsync(CreateRedisKey(key), valueString, expiration);
		}

		private RedisKey CreateRedisKey<TKey>(TKey key)
		{
			return new RedisKey(key!.ToString());
		}
	}
}

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Cache
{
	public interface ICache
	{
		Task Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expiration = null);
		Task<TValue?> Get<TKey, TValue>(TKey key);
		Task Remove<TKey>(TKey key);
	}
}

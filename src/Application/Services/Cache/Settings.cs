namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Cache
{
	public interface ICacheSettings : ISettings
	{
		string ConnectionString { get; }
	}
}

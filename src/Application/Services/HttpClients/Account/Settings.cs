namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account
{
	public interface IAccountHttpClientSettings : IHttpClientSettings
	{
		string Version { get; }
	}
}

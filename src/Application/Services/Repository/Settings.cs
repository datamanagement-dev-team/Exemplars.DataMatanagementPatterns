using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Repository
{
	public interface IRepositorySettings : ISettings
	{
		ResilienceSettings Resilience { get; }
	}
}

using BlueBrown.Data.DataManagementPatterns.Application.Services.Repository;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Resilience;

namespace BlueBrown.Data.DataManagementPatterns.Persistence.Services.Repository
{
	internal record RepositorySettings : IRepositorySettings
	{
		public const string ConfigurationKey = "repositoryconfiguration";

		public ResilienceSettings Resilience { get; init; } = new ResilienceSettings();

		public IReadOnlyCollection<string> Validate()
		{
			return new List<string>();
		}
	}
}

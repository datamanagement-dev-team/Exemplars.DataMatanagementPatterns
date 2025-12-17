using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Persistence
{
	public interface IPersistenceSettings : ISettings
	{
		string DataManagementPatternsConnectionString { get; }
	}

	internal record PersistenceSettings : IPersistenceSettings
	{
		public string DataManagementPatternsConnectionString { get; }

		public PersistenceSettings(string dataManagementPatternsConnectionString)
		{
			DataManagementPatternsConnectionString = dataManagementPatternsConnectionString;
		}

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(DataManagementPatternsConnectionString))
				errors.Add($"{nameof(IPersistenceSettings.DataManagementPatternsConnectionString)} should not be null");

			return errors;
		}
	}
}

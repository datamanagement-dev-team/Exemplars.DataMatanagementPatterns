namespace BlueBrown.Data.DataManagementPatterns.Persistence
{
	internal record SqlDatabaseSettings
	{
		public const string ConfigurationKey = "sqldatabaseconfiguration";

		public string ConnectionStringTemplate { get; init; } = string.Empty;
		//todo change this on actual implementation
		public SqlConnectionStringSettings DataManagementPatterns { get; init; } = new SqlConnectionStringSettings();

		internal record SqlConnectionStringSettings
		{
			public string Username { get; init; } = string.Empty;
			public string Password { get; init; } = string.Empty;
			public string DataSource { get; init; } = string.Empty;
			public string InitialCatalog { get; init; } = string.Empty;
			public bool IsReadOnly { get; init; }
		}
	}
}

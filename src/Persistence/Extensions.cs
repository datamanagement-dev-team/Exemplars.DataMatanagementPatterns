namespace BlueBrown.Data.DataManagementPatterns.Persistence
{
	internal static class Extensions
	{
		internal static object GetDatabaseValueOrDefault(this object? @object)
		{
			return @object ?? DBNull.Value;
		}
	}
}

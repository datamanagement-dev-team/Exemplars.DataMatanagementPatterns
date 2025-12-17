namespace BlueBrown.Data.DataManagementPatterns.Application
{
	public interface IValidatable
	{
		IReadOnlyCollection<string> Validate();
	}
}

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.Metrics
{
	public interface IMetrics
	{
		void IncreaseCounter(int amount, IReadOnlyDictionary<string, string> tags);
		void MeasureTime(long value, IReadOnlyDictionary<string, string> tags);
		void MeasureGauge(int value, IReadOnlyDictionary<string, string> tags);
	}
}

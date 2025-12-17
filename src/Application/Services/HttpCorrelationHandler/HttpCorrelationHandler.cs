namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpCorrelationHandler
{
	public interface IHttpCorrelationHandler
	{
		string? CorrelationId { get; }
		string? TraceId { get; }

		void SetCorrelationId(string correlationId);
		void SetTraceId(string traceId);
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class HttpCorrelationHandler : IHttpCorrelationHandler
	{
		public string? CorrelationId { get; private set; }
		public string? TraceId { get; private set; }

		public void SetCorrelationId(string correlationId)
		{
			CorrelationId = correlationId;
		}

		public void SetTraceId(string traceId)
		{
			TraceId = traceId;
		}
	}
}

namespace BlueBrown.Data.DataManagementPatterns.Application
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public record Response
	{
		public bool IsSuccess { get; }
		public int StatusCode { get; }
		public string? ErrorMessage { get; }
		public ErrorCode? ErrorCode { get; }

		protected Response(bool isSuccess, int statusCode, string? errorMessage = default, ErrorCode? errorCode = default)
		{
			IsSuccess = isSuccess;
			StatusCode = statusCode;
			ErrorMessage = errorMessage;
			ErrorCode = errorCode;
		}

		public static Response Create(int statusCode)
		{
			return new Response(isSuccess: true, statusCode: statusCode);
		}

		public static Response CreateErrored(int statusCode, string errorMessage, ErrorCode errorCode)
		{
			return new Response(isSuccess: false, statusCode: statusCode, errorMessage: errorMessage, errorCode: errorCode);
		}
	}

	public enum ErrorCode
	{
		GenericError,
		AuthenticationError
	}

	public record Response<TResult> : Response
	{
		public TResult? Result { get; }

		private Response(bool isSuccess, int statusCode, string? errorMessage = default, TResult? result = default, ErrorCode? errorCode = default)
			: base(isSuccess, statusCode, errorMessage, errorCode)
		{
			Result = result;
		}

		public static Response<TResult> Create(int statusCode, TResult result)
		{
			return new Response<TResult>(isSuccess: true, statusCode: statusCode, result: result);
		}

		public static new Response<TResult> CreateErrored(int statusCode, string errorMessage, ErrorCode errorCode)
		{
			return new Response<TResult>(isSuccess: false, statusCode: statusCode, errorMessage: errorMessage, errorCode: errorCode);
		}
	}
}

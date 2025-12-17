using BlueBrown.Data.DataManagementPatterns.Shared;

namespace BlueBrown.Data.DataManagementPatterns.Api.HttpResults
{
	public class HttpResult : IResult
	{
		private readonly Response _response;

		private HttpResult(Response response)
		{
			_response = response;
		}

		public static HttpResult Create(Response response)
		{
			return new HttpResult(response);
		}

		public async Task ExecuteAsync(HttpContext httpContext)
		{
			httpContext.Response.StatusCode = _response.StatusCode;
			await httpContext.Response.WriteAsJsonAsync(
				value: _response,
				options: Application.Extensions.JsonSerializerOptions,
				cancellationToken: httpContext.RequestAborted);
		}
	}

	public class HttpResult<TResult> : IResult
	{
		private readonly Response<TResult> _response;

		private HttpResult(Response<TResult> response)
		{
			_response = response;
		}

		public static HttpResult<TResult> Create(Response<TResult> response)
		{
			return new HttpResult<TResult>(response);
		}

		public async Task ExecuteAsync(HttpContext httpContext)
		{
			httpContext.Response.StatusCode = _response.StatusCode;
			await httpContext.Response.WriteAsJsonAsync(
				value: _response,
				options: Application.Extensions.JsonSerializerOptions,
				cancellationToken: httpContext.RequestAborted);
		}
	}
}

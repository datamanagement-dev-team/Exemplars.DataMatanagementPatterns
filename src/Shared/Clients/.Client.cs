using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System.Net;

namespace BlueBrown.Data.DataManagementPatterns.Shared.Clients
{
	public abstract class Client : IDisposable
	{
		private readonly IRestClient _restClient;

		protected Client(
			string baseUrl,
			HttpClient? httpClient = default)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
				throw new ArgumentException("should not be null", nameof(baseUrl));

			if (httpClient is not null)
			{
				httpClient.BaseAddress = new Uri(baseUrl);

				_restClient = new RestClient(
					httpClient: httpClient,
					disposeHttpClient: true,
					configureSerialization: _serializerConfig =>
					{
						var settings = new JsonSerializerSettings
						{
							Converters = new List<JsonConverter>
							{
								new StringEnumConverter()
							}
						};
						_serializerConfig.UseNewtonsoftJson(settings);
					});
			}
			else
				_restClient = new RestClient(
					baseUrl: new Uri(baseUrl),
					configureSerialization: _serializerConfig =>
					{
						var settings = new JsonSerializerSettings
						{
							Converters = new List<JsonConverter>
							{
								new StringEnumConverter()
							}
						};
						_serializerConfig.UseNewtonsoftJson(settings);
					});
		}

		public void Dispose()
		{
			_restClient.Dispose();
		}

		protected async Task<Response<TResult>> ExecuteWithResponse<TResult>(string resource, IReadOnlyDictionary<string, string>? parameters = default, IReadOnlyDictionary<string, string>? headers = default, CancellationToken cancellationToken = default)
		{
			try
			{
				var restRequest = new RestRequest(resource);

				parameters = parameters ?? new Dictionary<string, string>();

				foreach (var parameter in parameters)
					restRequest.AddQueryParameter(parameter.Key, parameter.Value);

				headers = headers ?? new Dictionary<string, string>();

				foreach (var header in headers)
					restRequest.AddHeader(header.Key, header.Value);

				var restResponse = await _restClient.ExecuteAsync<Response<TResult>>(restRequest, cancellationToken);

				var exception = restResponse.ErrorException;

				if (exception is not null)

				{
					var errorCode = restResponse.StatusCode == HttpStatusCode.Unauthorized
						? ErrorCode.AuthenticationError
						: ErrorCode.GenericError;

					return Response<TResult>.CreateErrored(statusCode: (int)restResponse.StatusCode, restResponse.ErrorMessage ?? exception.Message, errorCode: errorCode);
				}

				return restResponse.Data!;
			}
			catch (Exception exception)
			{
				return Response<TResult>.CreateErrored(statusCode: StatusCodes.Status500InternalServerError, errorMessage: exception.Message, errorCode: ErrorCode.GenericError);
			}
		}

		protected async Task<Response> Execute<TResult>(string resource, IReadOnlyDictionary<string, string>? parameters = default, IReadOnlyDictionary<string, string>? headers = default, CancellationToken cancellationToken = default)
		{
			try
			{
				var restRequest = new RestRequest(resource);

				parameters = parameters ?? new Dictionary<string, string>();

				foreach (var parameter in parameters)
					restRequest.AddQueryParameter(parameter.Key, parameter.Value);

				headers = headers ?? new Dictionary<string, string>();

				foreach (var header in headers)
					restRequest.AddHeader(header.Key, header.Value);

				var restResponse = await _restClient.ExecuteAsync<Response<TResult>>(restRequest, cancellationToken);

				var exception = restResponse.ErrorException;

				if (exception is not null)
				{
					var errorCode = restResponse.StatusCode == HttpStatusCode.Unauthorized
						? ErrorCode.AuthenticationError
						: ErrorCode.GenericError;

					return Response.CreateErrored(statusCode: (int)restResponse.StatusCode, restResponse.ErrorMessage ?? exception.Message, errorCode: errorCode);
				}

				return restResponse.Data!;
			}
			catch (Exception exception)
			{
				return Response.CreateErrored(statusCode: StatusCodes.Status500InternalServerError, errorMessage: exception.Message, errorCode: ErrorCode.GenericError);
			}
		}

		protected async Task<Response<TResult>> ExecuteWithResponse<TRequest, TResult>(string resource, Method httpMethod, TRequest request, IReadOnlyDictionary<string, string>? parameters = default, IReadOnlyDictionary<string, string>? headers = default, CancellationToken cancellationToken = default)
			where TRequest : class
		{
			try
			{
				var restRequest = new RestRequest(resource);

				restRequest.AddJsonBody(JsonConvert.SerializeObject(request));

				parameters = parameters ?? new Dictionary<string, string>();

				foreach (var parameter in parameters)
					restRequest.AddQueryParameter(parameter.Key, parameter.Value);

				headers = headers ?? new Dictionary<string, string>();

				foreach (var header in headers)
					restRequest.AddHeader(header.Key, header.Value);

				var restResponse = await _restClient.ExecuteAsync<Response<TResult>>(restRequest, httpMethod, cancellationToken);

				var exception = restResponse.ErrorException;

				if (exception is not null)
				{
					var errorCode = restResponse.StatusCode == HttpStatusCode.Unauthorized
						? ErrorCode.AuthenticationError
						: ErrorCode.GenericError;

					return Response<TResult>.CreateErrored(statusCode: (int)restResponse.StatusCode, restResponse.ErrorMessage ?? exception.Message, errorCode: errorCode);
				}

				return restResponse.Data!;
			}
			catch (Exception exception)
			{
				return Response<TResult>.CreateErrored(statusCode: StatusCodes.Status500InternalServerError, errorMessage: exception.Message, errorCode: ErrorCode.GenericError);
			}
		}

		protected async Task<Response> Execute<TRequest>(string resource, Method httpMethod, TRequest request, IReadOnlyDictionary<string, string>? parameters = default, IReadOnlyDictionary<string, string>? headers = default, CancellationToken cancellationToken = default)
			where TRequest : class
		{
			try
			{
				var restRequest = new RestRequest(resource);

				restRequest.AddJsonBody(JsonConvert.SerializeObject(request));

				parameters = parameters ?? new Dictionary<string, string>();

				foreach (var parameter in parameters)
					restRequest.AddQueryParameter(parameter.Key, parameter.Value);

				headers = headers ?? new Dictionary<string, string>();

				foreach (var header in headers)
					restRequest.AddHeader(header.Key, header.Value);

				var restResponse = await _restClient.ExecuteAsync<Response>(restRequest, httpMethod, cancellationToken);

				var exception = restResponse.ErrorException;

				if (exception is not null)
				{
					var errorCode = restResponse.StatusCode == HttpStatusCode.Unauthorized
						? ErrorCode.AuthenticationError
						: ErrorCode.GenericError;

					return Response.CreateErrored(statusCode: (int)restResponse.StatusCode, restResponse.ErrorMessage ?? exception.Message, errorCode: errorCode);
				}

				return restResponse.Data!;
			}
			catch (Exception exception)
			{
				return Response.CreateErrored(statusCode: StatusCodes.Status500InternalServerError, errorMessage: exception.Message, errorCode: ErrorCode.GenericError);
			}
		}
	}
}

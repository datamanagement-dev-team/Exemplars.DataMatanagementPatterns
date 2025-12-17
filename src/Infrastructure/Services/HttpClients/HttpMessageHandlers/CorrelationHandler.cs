using BlueBrown.Data.DataManagementPatterns.Application;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpCorrelationHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.HttpMessageHandlers
{
	internal class CorrelationHttpMessageHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CorrelationHttpMessageHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
		{
			var httpCorrelationHandler = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IHttpCorrelationHandler>();
			var correlationId = httpCorrelationHandler.CorrelationId;
			request.Headers.Add(LoggingConstants.CorrelationId, correlationId);
			var traceId = httpCorrelationHandler.TraceId;
			request.Headers.Add(LoggingConstants.TraceId, traceId);

			return await base.SendAsync(request, cancellationToken);
		}
	}
}

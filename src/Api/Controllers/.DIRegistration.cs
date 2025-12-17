using BlueBrown.Data.DataManagementPatterns.Api.ActionResults;
using BlueBrown.Data.DataManagementPatterns.Api.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BlueBrown.Data.DataManagementPatterns.Api.Controllers
{
	internal static class DIRegistration
	{
		internal static void RegisterControllers(this IServiceCollection services)
		{
			services
				.AddControllers(options =>
				{
					options.Filters.Add<LogAndMetricsFilter>();
				})
				.AddNewtonsoftJson(_options =>
				{
					_options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

					_options.SerializerSettings.Converters = new List<JsonConverter>
					{
						new StringEnumConverter()
					};

					_options.SerializerSettings.Error = (object? sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) =>
					{
						var exception = args.ErrorContext.Error;

						var path = args.ErrorContext.Path;

						var member = args.ErrorContext.Member;

						var logger = Infrastructure.Extensions.CreateLogger<Program>();

						logger.LogError(exception, "path: [{0}] - member: [{1}]", path, member);
					};
				})
				.ConfigureApiBehaviorOptions(_options =>
				{
					_options.InvalidModelStateResponseFactory = (_actionContext) =>
					{
						var loggerFactory = _actionContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();

						var logger = loggerFactory.CreateLogger<InvalidModelStateActionResult>();

						return new InvalidModelStateActionResult(logger);
					};

					_options.SuppressModelStateInvalidFilter = true;
				});
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Application;
using NJsonSchema;
using NSwag;
using System.Net.Mime;
using System.Reflection;

namespace BlueBrown.Data.DataManagementPatterns.Api.OpenApi
{
	internal static class DIRegistration
	{
		internal static void RegisterOpenApi(this IServiceCollection services, IConfiguration configuration)
		{
			var settings = configuration.GetSection(OpenApiSettings.ConfigurationKey).Get<OpenApiSettings>()!;

			services.AddSingleton<IOpenApiSettings>(settings);
			services.AddSingleton<IValidatable>(settings);

			services.AddSwaggerDocument(_settings =>
			{
				var projectFullName = Assembly.GetExecutingAssembly().GetName().Name;

				_settings.SchemaSettings.SchemaType = SchemaType.OpenApi3;

				_settings.DocumentName = $"{projectFullName} documentation";

				_settings.PostProcess = _openApiDocument =>
				{
					var projectName = projectFullName?
							.Replace("BlueBrown.", "")
							.Replace(".Api", "");

					_openApiDocument.Info = new OpenApiInfo
					{
						Description = $"{projectName} documentation",
						Title = projectName,
						Version = "1.0.0"
					};

					_openApiDocument.Consumes = new string[]
					{
						MediaTypeNames.Application.Json
					};

					_openApiDocument.Produces = new string[]
					{
						MediaTypeNames.Application.Json
					};
				};
			});
		}

		internal static void UseOpenApi(this IApplicationBuilder builder)
		{
			var settings = builder.ApplicationServices.GetRequiredService<IOpenApiSettings>();

			NSwagApplicationBuilderExtensions.UseOpenApi(builder);

			builder.UseSwaggerUi(_swaggerUi3Settings =>
			{
				_swaggerUi3Settings.Path = settings.Url;

				var projectFullName = Assembly.GetExecutingAssembly().GetName().Name;
				_swaggerUi3Settings.DocumentTitle = $"{projectFullName} documentation";
			});
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Api;

var logger = BlueBrown.Data.DataManagementPatterns.Infrastructure.Extensions.CreateLogger<Program>();

try
{
	var webApplicationBuilder = WebApplication.CreateBuilder(args);

	webApplicationBuilder.Configuration.Configure(webApplicationBuilder.Environment.EnvironmentName);

	webApplicationBuilder.Host.Configure(webApplicationBuilder.Configuration);

	webApplicationBuilder.Logging.Configure();

	webApplicationBuilder.Services.Configure(webApplicationBuilder.Configuration);

	if (!webApplicationBuilder.Environment.IsDevelopment())
	{
		var urls = webApplicationBuilder.Configuration.GetValue<string>("ASPNETCORE_URLS")!;

		webApplicationBuilder.WebHost.UseUrls(urls);
	}

	var webApplication = webApplicationBuilder.Build();

	logger.LogInformation("application built");

	webApplication.Configure();

	webApplication.Lifetime.Configure(webApplication.Services);

	await webApplication.RunAsync();
}
catch (Exception exception)
{
	logger.LogCritical(exception, string.Empty);

	throw;
}
finally
{
	BlueBrown.Data.DataManagementPatterns.Infrastructure.Extensions.ShutdownLogging();
}

public partial class Program { }

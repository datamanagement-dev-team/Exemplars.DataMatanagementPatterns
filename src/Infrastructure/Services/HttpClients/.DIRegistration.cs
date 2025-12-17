using Autofac;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Account;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.Fund;
using BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients.HttpMessageHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.HttpClients
{
	internal static class DIRegistration
	{
		internal static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
		{
			services.RegisterAccountHttpClient(configuration);
			services.RegisterFundHttpClient(configuration);
		}

		internal static void RegisterHttpClients(this ContainerBuilder builder)
		{
			builder.RegisterHttpMessageHandlers();
		}
	}
}

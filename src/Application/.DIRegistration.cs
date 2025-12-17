using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpCorrelationHandler;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Processor;
using BlueBrown.Data.DataManagementPatterns.Application.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace BlueBrown.Data.DataManagementPatterns.Application
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public static class DIRegistration
	{
		public static void RegisterApplicationServices(this IServiceCollection services)
		{
			Extensions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		}
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ApplicationDIRegistrationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterHttpCorrelationHandler();
			builder.RegisterHttpClients();
			builder.RegisterRepository();
			builder.RegisterProcessor();
		}
	}
}

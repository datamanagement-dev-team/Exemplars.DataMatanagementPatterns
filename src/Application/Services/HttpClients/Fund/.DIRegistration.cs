using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund.Decorators;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class DIRegistration
	{
		internal static void RegisterFundHttpClient(this ContainerBuilder builder)
		{
			builder.RegisterDecorator<FundHttpClientResilienceDecorator, IFundHttpClient>();
		}
	}
}

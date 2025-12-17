using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account.Decorators;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class DIRegistration
	{
		internal static void RegisterAccountHttpClient(this ContainerBuilder builder)
		{
			builder.RegisterDecorator<AccountHttpClientResilienceDecorator, IAccountHttpClient>();
		}
	}
}

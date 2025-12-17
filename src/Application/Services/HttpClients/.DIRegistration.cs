using Autofac;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Account;
using BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients.Fund;

namespace BlueBrown.Data.DataManagementPatterns.Application.Services.HttpClients
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class DIRegistration
	{
		internal static void RegisterHttpClients(this ContainerBuilder builder)
		{
			builder.RegisterAccountHttpClient();
			builder.RegisterFundHttpClient();
		}
	}
}

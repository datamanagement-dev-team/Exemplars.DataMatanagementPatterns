namespace BlueBrown.Data.DataManagementPatterns.Api.Middlewares
{
	internal static class DIRegistration
	{
		internal static void RegisterMiddlewares(this IServiceCollection services)
		{
			services.AddTransient<ExceptionHandlingMiddleware>();
		}
	}
}

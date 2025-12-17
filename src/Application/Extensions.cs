using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlueBrown.Data.DataManagementPatterns.Application
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public static class Extensions
	{
		public static IDisposable BeginScope(this ILogger logger, string key, object value)
		{
			return logger.BeginScope(new KeyValuePair<string, object>(key, value))!;
		}
		public static bool IsSystemicEndpoint(string requestPath)
		{
			if (string.IsNullOrEmpty(requestPath))
			{
				return false;
			}
			return requestPath.Contains("swagger")
				|| requestPath.EndsWith("/metrics")
				|| requestPath.Contains("/healthz")
				|| requestPath.Equals("/favicon.ico")
				|| requestPath.Equals("/");
		}
		public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		};
		public static string GetTypeName(this Type type)
		{
			if (!type.IsGenericType)
				return type.Name.Replace("<", "_").Replace(">", "_");
			else
			{
				var typeNameWithoutBackTick = type.Name.Replace($"`{type.GenericTypeArguments.Length}", "");
				var typeNameOfGenericTypeArguments = type.GenericTypeArguments.Select(_type => GetTypeName(_type));
				var typeNameOfGenericTypeArgumentsWithDiamondBrackets = $"<{typeNameOfGenericTypeArguments.Aggregate((_typeName1, _typeName2) => $"{_typeName1}, {_typeName2}")}>";
				return $"{typeNameWithoutBackTick}{typeNameOfGenericTypeArgumentsWithDiamondBrackets}".Replace("<", "_").Replace(">", "_");
			}
		}
	}
}

using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Api.OpenApi
{
	internal interface IOpenApiSettings : ISettings
	{
		string Url { get; }
	}

	internal record OpenApiSettings : IOpenApiSettings
	{
		public const string ConfigurationKey = "openapiconfiguration";

		public string Url { get; set; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(Url))
				errors.Add($"{nameof(IOpenApiSettings.Url)} should not be null");

			if (!Url.StartsWith("/"))
				errors.Add($"{nameof(IOpenApiSettings.Url)} should not be null");

			return errors;
		}
	}
}

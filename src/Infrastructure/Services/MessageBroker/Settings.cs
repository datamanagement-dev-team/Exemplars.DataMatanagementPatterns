using BlueBrown.Data.DataManagementPatterns.Application;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.MessageBroker
{
	internal record MessageBrokerSettings : ISettings
	{
		public const string ConfigurationKey = "messagebrokerconfiguration";

		public bool Enabled { get; init; }
		public string Username { get; init; } = string.Empty;
		public string Password { get; init; } = string.Empty;
		public string Endpoint { get; init; } = string.Empty;

		public IReadOnlyCollection<string> Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrWhiteSpace(Username))
				errors.Add($"{nameof(MessageBrokerSettings.Username)} should not be null");

			if (string.IsNullOrWhiteSpace(Password))
				errors.Add($"{nameof(MessageBrokerSettings.Password)} should not be null");

			if (string.IsNullOrWhiteSpace(Endpoint))
				errors.Add($"{nameof(MessageBrokerSettings.Endpoint)} should not be null");

			return errors;
		}
	}
}

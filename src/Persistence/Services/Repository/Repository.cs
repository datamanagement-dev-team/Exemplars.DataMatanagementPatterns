using BlueBrown.Data.DataManagementPatterns.Application.Services.Repository;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BlueBrown.Data.DataManagementPatterns.Persistence.Services.Repository
{
	internal class Repository : IRepository
	{
		private readonly IPersistenceSettings _settings;

		public Repository(IPersistenceSettings settings)
		{
			_settings = settings;
		}

		public async Task<object> GetCustomer(long customerId, CancellationToken cancellationToken = default)
		{
			using var connection = new SqlConnection(_settings.DataManagementPatternsConnectionString);

			var result = await connection.QueryAsync<dynamic>(new CommandDefinition(
				commandText:
					"""
					select * from dbo.Customers c
					where c.CustomerId = @customerId
					""",
				parameters: new
				{
					customerId
				},
				commandType: CommandType.StoredProcedure,
				cancellationToken: cancellationToken));

			return result.ToList();
		}
	}
}

using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;
using ILogger = Serilog.ILogger;

namespace ContentManagement.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IRelationalDBConnection _sqlConnection;
        private readonly ILogger _logger;

        public PersonRepository(IRelationalDBConnection sqlConnection, ILogger logger)
        {
            _sqlConnection = sqlConnection;
            _logger = logger;
        }

        public async Task<PersonModel> AddPerson(PersonModel person)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", person.UserName, DbType.String);
            parameters.Add("@DisplayName", person.DisplayName, DbType.String);
            parameters.Add("@Role", person.Role, DbType.String);
            
            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    var result = await connection.QuerySingleOrDefaultAsync<PersonModel>("dbo.usp_AddPerson", parameters, commandType: CommandType.StoredProcedure);

                    if (result == null)
                    {
                        return new PersonModel();
                    }
                    else
                    {
                        person.Id = result.Id;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Information(ex.Message);
                    throw;
                }
                
            }
            await Log.CloseAndFlushAsync();
            return person;
        }

        public async Task<bool> PersonExists(string username)
        {
            bool exists = false;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", username, DbType.String);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                exists = await connection.QuerySingleOrDefaultAsync<bool>("dbo.usp_PersonExists", parameters, commandType: CommandType.StoredProcedure);
            }

            return exists;
        }
    }
}

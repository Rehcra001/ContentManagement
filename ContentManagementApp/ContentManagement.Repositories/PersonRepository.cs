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

        public async Task<IEnumerable<PersonModel>> GetPeople()
        {
            IEnumerable<PersonModel> people = new List<PersonModel>();
            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                people = await connection.QueryAsync<PersonModel>("dbo.usp_GetPeople", commandType: CommandType.StoredProcedure);
            }
            return people;
        }

        /// <summary>
        /// Get people synchronously
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public IEnumerable<PersonModel> GetPeople(string str = "")
        {
            IEnumerable<PersonModel> people = new List<PersonModel>();
            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                people = connection.Query<PersonModel>("dbo.usp_GetPeople", commandType: CommandType.StoredProcedure);
            }
            return people;
        }

        public async Task<PersonModel> GetPerson(string username)
        {
            PersonModel person = new PersonModel();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", username, DbType.String);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                person = await connection.QuerySingleAsync<PersonModel>("dbo.GetPerson", parameters, commandType: CommandType.StoredProcedure);
            }
            return person;
        }

        public async Task<int> GetPersonId(string username)
        {
            int id = 0;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", username, DbType.String);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                id = await connection.QuerySingleAsync<int>("dbo.usp_GetPersonIdFromUserName", parameters, commandType: CommandType.StoredProcedure);
            }
            return id;
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

        public async Task<bool> UpdatePerson(PersonModel person)
        {
            bool updated = false;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", person.UserName, DbType.String);
            parameters.Add("@DisplayName", person.DisplayName, DbType.String);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                updated = await connection.QuerySingleAsync<bool>("dbo.usp_UpdatePerson", parameters, commandType: CommandType.StoredProcedure);
            }

            return updated;
        }
    }
}

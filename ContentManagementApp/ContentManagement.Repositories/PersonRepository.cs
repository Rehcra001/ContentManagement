using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ContentManagement.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private IRelationalDBConnection _sqlConnection;

        public PersonRepository(IRelationalDBConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public async Task<PersonModel> AddPerson(PersonModel person)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", person.UserName, DbType.String);
            parameters.Add("@DisplayName", person.DisplayName, DbType.String);
            parameters.Add("@Role", person.Role, DbType.String);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
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

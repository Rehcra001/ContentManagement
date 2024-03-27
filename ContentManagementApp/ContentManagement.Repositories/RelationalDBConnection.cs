using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ContentManagement.Repositories
{
    public class RelationalDBConnection : IRelationalDBConnection
    {
        private readonly IConfiguration _config;

        public RelationalDBConnection(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection sqlConnection()
        {
            return new SqlConnection(_config.GetConnectionString("ContentManagementDB"));
        }
    }
}

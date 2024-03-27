using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Repositories
{
    public interface IRelationalDBConnection
    {
        SqlConnection sqlConnection();
    }
}

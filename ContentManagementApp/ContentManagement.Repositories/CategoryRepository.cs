using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using ILogger = Serilog.ILogger;
using System.Data;

namespace ContentManagement.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IRelationalDBConnection _sqlConnection;
        private readonly ILogger _logger;

        public CategoryRepository(IRelationalDBConnection sqlConnection,
                                  ILogger logger)
        {
            _sqlConnection = sqlConnection;
            _logger = logger;
        }

        public async Task<CategoryModel> AddCategory(CategoryModel category)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", category.Name, DbType.String);
            parameters.Add("@Description", category.Description, DbType.String);
            parameters.Add("@IsPublished", category.IsPublished, DbType.Boolean);
            parameters.Add("@CreatedOn", category.CreatedOn, DbType.DateTime2);
            if (category.LastModified == default)
            {
                parameters.Add("@LastModified", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@LastModified", category.LastModified, DbType.DateTime2);
            }
            if (category.PublishedOn == default)
            {
                parameters.Add("@PublishedOn", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@PublishedOn", category.PublishedOn, DbType.DateTime2);
            }

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    var result = await connection.QuerySingleOrDefaultAsync<CategoryModel>("dbo.usp_AddCategory", parameters, commandType: CommandType.StoredProcedure);

                    if (result != null)
                    {
                        category.Id = result.Id;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return new CategoryModel();
                }
                
            }

            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            bool deleted = false;

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    bool? result = await connection.QuerySingleOrDefaultAsync<bool>("dbo.usp_DeleteCategory", parameters, commandType: CommandType.StoredProcedure);
                    if (result != null)
                    {
                        deleted = (bool)result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return false;
                }
            }

            return deleted;
        }

        public async Task<IEnumerable<CategoryModel>> GetCategories()
        {
            IEnumerable<CategoryModel> categories = new List<CategoryModel>();

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    categories = await connection.QueryAsync<CategoryModel>("dbo.usp_GetCategories", param: null, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return categories;
                }
            }

            return categories;
        }

        public async Task<CategoryModel> GetCategory(int id)
        {
            CategoryModel category = new CategoryModel();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    category = await connection.QuerySingleAsync<CategoryModel>("dbo.usp_GetCategory", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return category;
                }
            }
            return category;
        }

        public async Task<bool> UpdateCategory(CategoryModel category)
        {
            bool updated = false;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", category.Id, DbType.Int32);
            parameters.Add("@Name", category.Name, DbType.String);
            parameters.Add("@Description", category.Description, DbType.String);
            parameters.Add("@IsPublished", category.IsPublished, DbType.Boolean);
            parameters.Add("@CreatedOn", category.CreatedOn, DbType.DateTime2);
            if (category.LastModified == default)
            {
                parameters.Add("@LastModified", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@LastModified", category.LastModified, DbType.DateTime2);
            }
            if (category.PublishedOn == default)
            {
                parameters.Add("@PublishedOn", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@PublishedOn", category.PublishedOn, DbType.DateTime2);
            }

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    updated = await connection.QuerySingleAsync<bool>("dbo.usp_UpdateCategory", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return false;
                }
            }
            return updated;
        }
    }
}

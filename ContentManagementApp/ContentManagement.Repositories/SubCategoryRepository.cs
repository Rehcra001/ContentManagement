using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ILogger = Serilog.ILogger;

namespace ContentManagement.Repositories
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly IRelationalDBConnection _sqlConnection;
        private readonly ILogger _logger;

        public SubCategoryRepository(IRelationalDBConnection sqlConnection, ILogger logger)
        {
            _sqlConnection = sqlConnection;
            _logger = logger;
        }

        public async Task<SubCategoryModel> AddSubCategory(SubCategoryModel subCategory)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", subCategory.Name, DbType.String);
            parameters.Add("@Description", subCategory.Description, DbType.String);
            parameters.Add("@IsPublished", subCategory.IsPublished, DbType.Boolean);
            parameters.Add("@CreatedOn", subCategory.CreatedOn, DbType.DateTime2);
            if (subCategory.LastModified == default)
            {
                parameters.Add("@LastModifiedOn", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@LastModifiedOn", subCategory.LastModified, DbType.DateTime2);
            }
            if (subCategory.PublishedOn == default)
            {
                parameters.Add("@PublishedOn", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@PublishedOn", subCategory.PublishedOn, DbType.DateTime2);
            }

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    var result = await connection.QuerySingleOrDefaultAsync<SubCategoryModel>("dbo.usp_AddSubCategory", parameters, commandType: CommandType.StoredProcedure);

                    if (result != null)
                    {
                        subCategory.Id = result.Id;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return new SubCategoryModel();
                }

            }

            return subCategory;
        }

        public async Task<bool> CanDeleteSubCategory(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@SubCategoryId", id, DbType.Int32);
            bool canDelete = false;

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    canDelete = await connection.QuerySingleAsync<bool>("dbo.usp_CanDeleteSubCategory", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {

                    _logger.Error(ex, ex.Message);
                    return false;
                }
            }

            return canDelete;
        }

        public async Task<bool> DeleteSubCategory(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            bool deleted = false;

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    bool? result = await connection.QuerySingleOrDefaultAsync<bool>("dbo.usp_DeleteSubCategory", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<IEnumerable<SubCategoryModel>> GetSubCategories()
        {
            IEnumerable<SubCategoryModel> subCategories = new List<SubCategoryModel>();

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    subCategories = await connection.QueryAsync<SubCategoryModel>("dbo.usp_GetSubCategories", param: null, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return subCategories;
                }
            }

            return subCategories;
        }

        public async Task<SubCategoryModel> GetSubCategory(int id)
        {
            SubCategoryModel subCategory = new SubCategoryModel();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    subCategory = await connection.QuerySingleAsync<SubCategoryModel>("dbo.usp_GetSubCategory", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return subCategory;
                }
            }
            return subCategory;
        }

        public async Task<bool> UpdateSubCategory(SubCategoryModel subCategory)
        {
            bool updated = false;
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", subCategory.Id, DbType.Int32);
            parameters.Add("@Name", subCategory.Name, DbType.String);
            parameters.Add("@Description", subCategory.Description, DbType.String);
            parameters.Add("@IsPublished", subCategory.IsPublished, DbType.Boolean);
            parameters.Add("@CreatedOn", subCategory.CreatedOn, DbType.DateTime2);
            if (subCategory.LastModified == default)
            {
                parameters.Add("@LastModified", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@LastModified", subCategory.LastModified, DbType.DateTime2);
            }
            if (subCategory.PublishedOn == default)
            {
                parameters.Add("@PublishedOn", DBNull.Value, DbType.DateTime2);
            }
            else
            {
                parameters.Add("@PublishedOn", subCategory.PublishedOn, DbType.DateTime2);
            }

            using (SqlConnection connection = _sqlConnection.sqlConnection())
            {
                try
                {
                    updated = await connection.QuerySingleAsync<bool>("dbo.usp_UpdateSubCategory", parameters, commandType: CommandType.StoredProcedure);
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

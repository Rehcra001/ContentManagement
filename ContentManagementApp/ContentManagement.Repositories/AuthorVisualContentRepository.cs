using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using ILogger = Serilog.ILogger;

namespace ContentManagement.Repositories
{
    public class AuthorVisualContentRepository : IAuthorVisualContentRepository
    {
        private readonly IRelationalDBConnection _sqlConnection;
        private readonly ILogger _logger;

        public AuthorVisualContentRepository(IRelationalDBConnection sqlConnection, ILogger logger)
        {
            _sqlConnection = sqlConnection;
            _logger = logger;
        }

        public async Task<AuthorVisualContentModel?> AddAuthorVisualContent(AuthorVisualContentModel authorVisualContent)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@AuthorId", authorVisualContent.AuthorId, DbType.Int32);
                parameters.Add("@Name", authorVisualContent.Name, DbType.String);
                parameters.Add("@Description", authorVisualContent.Description, DbType.String);
                parameters.Add("@FileName", authorVisualContent.FileName, DbType.String);
                parameters.Add("@VisualContentType", authorVisualContent.VisualContentType, DbType.String);
                parameters.Add("@IsHttpLink", authorVisualContent.IsHttpLink, DbType.Boolean);

                using (SqlConnection connection = _sqlConnection.sqlConnection())
                {
                    var content = await connection.QuerySingleOrDefaultAsync<AuthorVisualContentModel>("dbo.usp_AddAuthorVisualContent",
                                                                                              parameters,
                                                                                              commandType: CommandType.StoredProcedure);
                    if (content is null || content.Id == 0)
                    {
                        return null;
                    }
                    authorVisualContent = content;
                }
                return authorVisualContent;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CanDeleteAuthorVisualContent(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AuthorVisualContentId", id, DbType.Int32);
            bool canDelete = false;

            try
            {
                using (SqlConnection connection = _sqlConnection.sqlConnection())
                {
                    canDelete = await connection.QuerySingleAsync<bool>("dbo.usp_CanDeleteAuthorVisualContent",
                                                                        parameters,
                                                                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception("Unexpected Error");
            }
            return canDelete;
        }

        public async Task<bool> DeleteAuthorVisualContent(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            bool deleted = false;

            try
            {
                using (SqlConnection connection = _sqlConnection.sqlConnection())
                {
                    deleted = await connection.QuerySingleAsync<bool>("dbo.usp_DeleteAuthorVisualContent",
                                                                      parameters,
                                                                      commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
            return deleted;
        }

        public async Task<IEnumerable<AuthorVisualContentModel>> GetAllAuthorVisualContent(int authorId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AuthorId", authorId, DbType.Int32);
            IEnumerable<AuthorVisualContentModel> content = new List<AuthorVisualContentModel>();
            try
            {
                using (SqlConnection connection = _sqlConnection.sqlConnection())
                {
                    var result = await connection.QueryAsync<AuthorVisualContentModel>("dbo.usp_GetAllAuthorVisualContent",
                                                                                    parameters,
                                                                                    commandType: CommandType.StoredProcedure);

                    if (result is not null && result.Count() > 0)
                    {
                        content = result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }

            return content;
        }

        public async Task<AuthorVisualContentModel> GetAuthorVisualContent(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            AuthorVisualContentModel content = new AuthorVisualContentModel();

            try
            {
                using (SqlConnection connection = _sqlConnection.sqlConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<AuthorVisualContentModel>("dbo.usp_GetAuthorVisualContent",
                                                                                                      parameters,
                                                                                                      commandType: CommandType.StoredProcedure);

                    if (result is not null && result.Id != default)
                    {
                        content = result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
            return content;
        }

        public async Task<bool> UpdateAuthorVisualContent(AuthorVisualContentModel authorVisualContent)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", authorVisualContent.Id, DbType.Int32);
            parameters.Add("@AuthorId", authorVisualContent.AuthorId, DbType.Int32);
            parameters.Add("@Name", authorVisualContent.Name, DbType.String);
            parameters.Add("@Description", authorVisualContent.Description, DbType.String);
            parameters.Add("@FileName", authorVisualContent.FileName, DbType.String);
            parameters.Add("@VisualContentType", authorVisualContent.VisualContentType, DbType.String);
            parameters.Add("@IsHttpLink", authorVisualContent.IsHttpLink, DbType.Boolean);

            bool updated = false;

            try
            {
                using (SqlConnection connection = _sqlConnection.sqlConnection())
                {
                    updated = await connection.QuerySingleAsync<bool>("dbo.usp_UpdateAuthorVisualContent",
                                                                      parameters,
                                                                      commandType: CommandType.StoredProcedure);

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
            return updated;
        }
    }
}

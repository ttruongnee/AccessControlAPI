using AccessControlAPI.Database;
using AccessControlAPI.Models;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class UserFunctionRepository : IUserFunctionRepository
    {
        private readonly IOracleDb _oracleDb;
        public UserFunctionRepository(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }
        public bool AddFunctionsForUser(int userId, List<string> functionIds)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                try
                {
                    string sql = "insert into users_functions (user_id, function_id) values (:userId, :functionId)";
                    foreach (var functionId in functionIds)
                    {
                        conn.Execute(sql, new { userId, functionId });
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        //xoá toàn bộ quyền của user (trong bảng user_functions)
        public bool DeleteFunctionsFromUser(int userId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from users_functions where user_id = :userId";
                return conn.Execute(sql, new { userId }) > 0;
            }
        }

        public List<Function> GetFunctionsByUserId(int userId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = @"
                                SELECT f.*
                                FROM functions f
                                INNER JOIN users_functions uf ON f.id = uf.function_id
                                WHERE uf.user_id = :userId";

                return conn.Query<Function>(sql, new { userId }).ToList();
            }
        }
    }
}

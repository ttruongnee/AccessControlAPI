
using AccessControlAPI.Database;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class RoleFunctionRepository : IRoleFunctionRepository
    {
        private readonly IOracleDb _oracleDb;
        public RoleFunctionRepository(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }

        public bool DeleteFunctionsFromRole(int roleId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from roles_functions where role_id = :roleId";
                return conn.Execute(sql, new { roleId }) > 0;
            }
        }

        public List<Function> GetFunctionsByRoleId(int roleId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = @"
                                SELECT rf.ROLE_ID, f.*
                                FROM functions f
                                INNER JOIN roles_functions rf ON f.id = rf.function_id
                                WHERE rf.role_id = :roleId";

                return conn.Query<Function>(sql, new { roleId }).ToList();
            }
        }

        public bool UpdateFunctionsForRole(int roleId, List<string> functionIds)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // xóa quyền cũ
                        string deleteSql = "DELETE FROM roles_functions WHERE role_id = :roleId";
                        conn.Execute(deleteSql, new { roleId }, transaction: transaction);

                        // chèn quyền mới (nếu có)
                        if (functionIds != null && functionIds.Count > 0)
                        {
                            string insertSql = "INSERT INTO roles_functions (role_id, function_id) VALUES (:roleId, :functionId)";

                            foreach (var functionId in functionIds)
                            {
                                conn.Execute(insertSql, new { roleId, functionId = functionId.Trim().ToUpper() }, transaction: transaction);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                        //return false;
                    }
                }
            }
        }
    }
}

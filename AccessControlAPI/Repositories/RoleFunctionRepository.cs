
using AccessControlAPI.Database;
using AccessControlAPI.Models;
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
        public bool AddFunctionsForRole(int roleId, List<string> functionIds)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                try
                {
                    string sql = "insert into roles_functions (role_id, function_id) values (:roleId, :functionId)";
                    foreach (var functionId in functionIds)
                    {
                        conn.Execute(sql, new { roleId, functionId });
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool DeleteFunctionsFromRole(int roleId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from roles_functions where role_id = :roleId";
                return conn.Execute(sql, new { roleId }) > 0;
            }
        }

        public List<Function> GetFunctionIdsByRoleId(int roleId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = @"
                                SELECT f.*
                                FROM functions f
                                INNER JOIN roles_functions rf ON f.id = rf.function_id
                                WHERE rf.role_id = :roleId";

                return conn.Query<Function>(sql, new { roleId }).ToList();
            }
        }
    }
}

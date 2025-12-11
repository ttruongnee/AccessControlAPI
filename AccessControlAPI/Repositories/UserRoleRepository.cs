using AccessControlAPI.Database;
using AccessControlAPI.Models;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IOracleDb _oracleDb;
        public UserRoleRepository(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }

        public bool DeleteRolesFromUser(int userId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from users_roles where user_id = :userId";
                return conn.Execute(sql, new { userId }) > 0;
            }
        }

        public List<Role> GetRolesByUserId(int userId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = @"
                                SELECT r.*
                                FROM roles r
                                INNER JOIN users_roles ur ON r.id = ur.role_id
                                WHERE ur.user_id = :userId";

                return conn.Query<Role>(sql, new { userId }).ToList();
            }
        }

        public bool UpdateRolesForUser(int userId, List<int> roleIds)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // xóa vai trò cũ
                        string deleteSql = "DELETE FROM users_roles WHERE user_id = :userId";
                        conn.Execute(deleteSql, new { userId }, transaction: transaction);

                        // chèn vai trò mới (nếu có)
                        if (roleIds != null && roleIds.Count > 0)
                        {
                            string insertSql = "INSERT INTO users_roles (user_id, role_id) VALUES (:userId, :roleId)";

                            foreach (var roleId in roleIds)
                            {
                                conn.Execute(insertSql, new { userId, roleId }, transaction: transaction);
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

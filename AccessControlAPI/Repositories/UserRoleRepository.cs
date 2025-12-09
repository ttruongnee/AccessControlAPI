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
        public bool AddRolesForUser(int userId, List<int> roleIds)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                try
                {
                    string sql = "insert into users_roles (user_id, role_id) values (:userId, :roleId)";
                    foreach (var roleId in roleIds)
                    {
                        conn.Execute(sql, new { userId, roleId });
                    }
                    return true;
                }
                catch (Exception e)
                {

                    return false;
                }
            }
        }

        public bool DeleteRoleFromUser(int userId)
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
    }
}

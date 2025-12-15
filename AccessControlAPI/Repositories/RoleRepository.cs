using AccessControlAPI.Database;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IOracleDb _oracleDb;
        public RoleRepository(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }

        public int GetNextRoleId()
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "SELECT seq_role.NEXTVAL FROM dual";
                return conn.QuerySingle<int>(sql);
            }
        }

        public bool Create(Role role, out int newRoleId)
        {
            using(var conn = _oracleDb.GetConnection())
            {
                newRoleId = GetNextRoleId();
                string sql = "insert into roles (id, name) values (:id, :name)";
                return conn.Execute(sql, new {id = newRoleId, name = role.Name }) > 0;
            }
        }

        public bool Delete(int id)
        {
            using(var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from roles where id = :id";
                return conn.Execute(sql, new { id }) > 0;
            }
        }

        public List<Role> GetAll()
        {
            using(var conn = _oracleDb.GetConnection())
            { 
                string sql = "select * from roles";
                return conn.Query<Role>(sql).ToList();
            }
        }

        public Role GetById(int id)
        {
            using(var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from roles where id = :id";
                return conn.QueryFirstOrDefault<Role>(sql, new { id });
            }
        }

        public bool Update(int id, Role role)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "update roles set name = :name where id = :id";
                return conn.Execute(sql, new { name = role.Name, id }) > 0;
            }
        }
    }
}

using AccessControlAPI.Database;
using AccessControlAPI.Models;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IOracleDb _oracleDb;
        public UserRepository(IOracleDb oracleDb) 
        {
            _oracleDb = oracleDb;
        }
        public int GetNextUserId()
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "SELECT seq_user.NEXTVAL FROM dual";
                return conn.QuerySingle<int>(sql);
            }
        }

        public bool Create(User user, out int newUserId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                newUserId = GetNextUserId();
                string sql = "insert into users (id, username, password) values (:id, :username, :password)";
                return conn.Execute(sql, new { id= newUserId, username = user.Username, password = user.Password }) > 0;
            }
        }

        public bool Delete(int id)
        {
            using(var conn = _oracleDb.GetConnection())
            {
                string sql = "delete from users where id = :id";
                return conn.Execute(sql, new { id }) > 0;
            }
        }

        public List<User> GetAll()
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from users";
                return conn.Query<User>(sql).ToList();
            }
        }

        public User GetById(int id)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from users where id = :id";
                return conn.QueryFirstOrDefault<User>(sql, new { id });
            }
        }

        public User GetByUsername(string username)
        {
            using(var conn = _oracleDb.GetConnection())
            {
                string sql = "select * from users where username = :username";
                return conn.QueryFirstOrDefault<User>(sql, new { username });
            }
        }

        public bool Update(int id, User user)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                string sql = "update users set username = :username, password = :password where id = :id";
                return conn.Execute(sql, new { username = user.Username, password = user.Password, id }) > 0;
            }
        }
    }
}

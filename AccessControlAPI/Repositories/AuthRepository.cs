using AccessControlAPI.Database;
using AccessControlAPI.Models;
using AccessControlAPI.Utils;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IOracleDb _oracleDb;
        private readonly IUserRepository _userRepository;
        public AuthRepository(IOracleDb oracleDb, IUserRepository userRepository)
        {
            _oracleDb = oracleDb;
            _userRepository = userRepository;
        }
        public User Login(string username)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                var sql = "select * from users where username = :username";
                return conn.QueryFirstOrDefault<User>(sql, username);
            }
        }

        public bool Register(User user, out int newUserId)
        {
            using (var conn = _oracleDb.GetConnection())
            {
                newUserId = _userRepository.GetNextUserId();
                string sql = "insert into users (id, username, password) values (:id, :username, :password)";
                return conn.Execute(sql, new { id = newUserId, username = user.Username, password = user.Password }) > 0;
            }
        }
    }
}

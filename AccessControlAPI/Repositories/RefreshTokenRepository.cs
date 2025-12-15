using AccessControlAPI.Database;
using AccessControlAPI.Models;
using AccessControlAPI.Repositories.Interface;
using Dapper;

namespace AccessControlAPI.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IOracleDb _oracleDb;

        public RefreshTokenRepository(IOracleDb oracleDb)
        {
            _oracleDb = oracleDb;
        }

        public void Save(int userId, string token, DateTime expiresAt)
        {
            using var conn = _oracleDb.GetConnection();
            string sql = @"insert into refresh_tokens (user_id, token, expires_at) values (:userId, :token, :expiresAt)";

            conn.Execute(sql, new
            {
                userId,
                token,
                expiresAt
            });
        }

        public RefreshToken GetByToken(string token)
        {
            using var conn = _oracleDb.GetConnection();
            string sql = @"SELECT *
                   FROM refresh_tokens
                   WHERE token = :token";

            return conn.QueryFirstOrDefault<RefreshToken>(sql, new { token });
        }

        public void Revoke(string token)
        {
            using var conn = _oracleDb.GetConnection();
            string sql = @"UPDATE refresh_tokens
                   SET is_revoked = 1
                   WHERE token = :token";

            conn.Execute(sql, new { token });
        }
    }
}

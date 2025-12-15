using AccessControlAPI.Models;

namespace AccessControlAPI.Repositories
{
    public interface IRefreshTokenRepository
    {
        void Save(int userId, string token, DateTime expiresAt);
        RefreshToken GetByToken(string token);
        void Revoke(string token);
    }
}

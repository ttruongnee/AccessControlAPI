namespace AccessControlAPI.Services.Interface
{
    public interface IRefreshTokenService
    {
        bool RefreshAccessToken(string refreshToken, out string newAccessToken, out string newRefreshToken, out string message);
        void Logout(string refreshToken);
    }
}

using System.Security.Cryptography;

namespace AccessControlAPI.Utils
{
    public static class RefreshTokenHelper
    {
        public static string Generate()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}

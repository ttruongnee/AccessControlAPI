namespace AccessControlAPI.Utils
{
    public class ConfigurationHelper
    {
        private readonly IConfiguration _config;  //cho phép đọc dữ liệu từ appsettings.json
        private readonly EncryptHelper _encryptHelper; //dùng để giải mã 

        public ConfigurationHelper(IConfiguration config, EncryptHelper encryptHelper)
        {
            _config = config;
            _encryptHelper = encryptHelper;
        }

        //lấy chuỗi kết nối đã mã hoá
        public string GetEncryptedConnectionString()
        {
            return _config["EncryptedConnectionString"];
        }


        //lấy chuỗi kết nối đã giải mã
        public string GetDecryptedConnectionString()
        {
            string encrypted = GetEncryptedConnectionString();
            return _encryptHelper.Decrypt(encrypted);
        }

        //lấy chuỗi kết nối chưa mã hoá (không dùng trong thực tế)
        public string GetConnectionString()
        {
            return _config.GetConnectionString("Default");
        }
    }
}

using System.Security.Cryptography;
using System.Text;

namespace AccessControlAPI.Utils
{
    public class EncryptHelper
    {
        private readonly string _key;

        //Lấy chuỗi Key từ file appsettings.json (32 ký tự đối với AES-256)
        public EncryptHelper(IConfiguration config)
        {
            _key = config["Encryption:Key"];

            if (string.IsNullOrWhiteSpace(_key) || _key.Length != 32)
                throw new Exception("AES Key phải có 32 ký tự đối với AES-256.");
        }


        //mã hoá
        public string Encrypt(string plainText)
        {
            //tạo object AES với key đã cho và IV ngẫu nhiên
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);
                aes.GenerateIV(); // IV sẽ khác mỗi lần

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Ghi IV vào đầu dữ liệu trả về(để sau này cần giải mã thì đọc lại)
                    ms.Write(aes.IV, 0, aes.IV.Length); 

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        //giải mã
        public string Decrypt(string cipherText)
        {
            //lấy dữ liệu nhị phân bị mã hoá
            byte[] bytes = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);
                //tách 16 byte đầu lấy IV từ dữ liệu
                byte[] iv = new byte[16];
                Array.Copy(bytes, 0, iv, 0, 16); // lấy IV từ đầu file
                aes.IV = iv;

                //giải mã từ byte 17 trở đi
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(bytes, 16, bytes.Length - 16))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}

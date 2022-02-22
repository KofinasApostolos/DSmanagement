using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Class
{
    public static class PasswordHash
    {
        static PasswordHash() {}

        private static string hash = "f0xle@rn";

        public static string decryptPassword(string password)
        {
            try
            {
                byte[] data = Convert.FromBase64String(password); //to encrypt password
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripleDES.CreateDecryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        password = UTF8Encoding.UTF8.GetString(results);
                    }
                }
                return password;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static string encryptPassword(string password)
        {
            try
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(password); //to password plain text
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripleDES.CreateEncryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        password = Convert.ToBase64String(results, 0, results.Length);
                    }
                }
                return password;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}

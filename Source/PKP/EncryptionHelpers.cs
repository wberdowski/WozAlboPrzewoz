using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WozAlboPrzewoz
{
    public static class EncryptionHelpers
    {
        private static string passPhrase = "Portal Pasażera";

        public static string Encrypt(string plainText)
        {
            byte[] array = a();
            byte[] array2 = a();
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, array, 1000))
            {
                byte[] bytes2 = rfc2898DeriveBytes.GetBytes(32);
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.BlockSize = 256;
                    rijndaelManaged.Mode = CipherMode.CBC;
                    rijndaelManaged.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes2, array2))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(bytes, 0, bytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] inArray = array.Concat(array2).ToArray().Concat(memoryStream.ToArray())
                                    .ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(inArray).Replace("+", "scc2B").Replace("/", "scc2F");
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace("scc2B", "+").Replace("scc2F", "/");
            byte[] array = Convert.FromBase64String(cipherText);
            byte[] salt = array.Take(32).ToArray();
            byte[] rgbIV = array.Skip(32).Take(32).ToArray();
            byte[] array2 = array.Skip(64).Take(array.Length - 64).ToArray();
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passPhrase, salt, 1000))
            {
                byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.BlockSize = 256;
                    rijndaelManaged.Mode = CipherMode.CBC;
                    rijndaelManaged.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, rgbIV))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(array2))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                            {
                                byte[] array3 = new byte[array2.Length];
                                int count = cryptoStream.Read(array3, 0, array3.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(array3, 0, count);
                            }
                        }
                    }
                }
            }
        }

        //public static string GetMD5(string input)
        //{
        //    MD5 mD = MD5.Create();
        //    byte[] bytes = new UTF8Encoding().GetBytes(input);
        //    return BitConverter.ToString(mD.ComputeHash(bytes));
        //}

        private static byte[] a()
        {
            byte[] array = new byte[32];
            using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rNGCryptoServiceProvider.GetBytes(array);
                return array;
            }
        }
    }
}
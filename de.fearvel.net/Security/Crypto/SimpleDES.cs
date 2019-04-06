using System;
using System.Security.Cryptography;
using System.Text;

namespace de.fearvel.net.Security.Crypto
{
    /// <summary>
    /// Class for simple DES encryption
    /// output is decryptable in other programming languages
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class SimpleDES
    {
        /// <summary>
        /// Encrypts string
        /// </summary>
        /// <param name="source">string to be encrypted</param>
        /// <param name="key">password</param>
        /// <param name="programIv">initialization vector</param>
        /// <returns></returns>
        public static string Encrypt(string source, string key, string programIv)
        {
            var desCryptoProvider = new TripleDESCryptoServiceProvider();
            try
            {
                desCryptoProvider.Key = Encoding.UTF8.GetBytes(key);
                desCryptoProvider.IV = Encoding.UTF8.GetBytes(programIv);
                var byteBuff = Encoding.UTF8.GetBytes(source);
                string iv = Convert.ToBase64String(desCryptoProvider.IV);
                Console.WriteLine(@"iv: {0}", iv);
                string encoded =
                    Convert.ToBase64String(desCryptoProvider.CreateEncryptor()
                        .TransformFinalBlock(byteBuff, 0, byteBuff.Length));

                return encoded;
            }
            catch (Exception except)
            {
                Console.WriteLine(except + "\n\n" + except.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Decrypts string
        /// </summary>
        /// <param name="encryptedText"> encryptedText</param>
        /// <param name="key">password</param>
        /// <param name="programIv">initialization vector</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string key, string programIv)
        {
            var desCryptoProvider = new TripleDESCryptoServiceProvider();
            try
            {
                desCryptoProvider.Key = Encoding.UTF8.GetBytes(key);
                desCryptoProvider.IV = Encoding.UTF8.GetBytes(programIv);
                var byteBuff = Convert.FromBase64String(encryptedText);
                var plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor()
                    .TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                return plaintext;
            }
            catch (Exception except)
            {
                Console.WriteLine(except + "\n\n" + except.StackTrace);
                return null;
            }
        }
    }
}
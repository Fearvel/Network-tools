using System.Security.Cryptography;
using System.Text;

namespace de.fearvel.net.Security.Hash
{
    /// <summary>
    /// Class for Sha265 generation
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public static class Sha256
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringIn">input string</param>
        /// <param name="upperCase">boolean if true the hash will be uppercase</param>
        /// <returns>sha256 hash</returns>
        public static string GenerateSha256(string stringIn, bool upperCase = true)
        {
            string hashString;
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.Default.GetBytes(stringIn));
                hashString = Utils.ToHex(hash, upperCase);
            }

            return hashString;
        }
    }
}
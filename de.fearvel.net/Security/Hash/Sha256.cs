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
        /// Generates a Sha256 hash of a string
        /// uses System.Security.Cryptography
        /// </summary>
        /// <param name="stringIn">input string</param>
        /// <param name="upperCase">boolean if true the hash will be uppercase</param>
        /// <returns>sha256 hash</returns>
        public static string GenerateSha256(string stringIn, bool upperCase = true)
        {
            return GenerateSha256(Encoding.Default.GetBytes(stringIn), upperCase);
        }

        /// <summary>
        /// Generates a Sha256 hash of a bytearray
        /// uses System.Security.Cryptography
        /// </summary>
        /// <param name="input">bytearray containing the information that will be hashed</param>
        /// <param name="upperCase">boolean if true the hash will be uppercase</param>
        /// <returns>sha256 hash</returns>
        public static string GenerateSha256(byte[] input, bool upperCase = true)
        {
            string hashString;
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(input);
                hashString = Utils.ToHex(hash, upperCase);
            }
            return hashString;
        }
    }
}
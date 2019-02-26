using System.Text;

namespace de.fearvel.net.Security.Hash
{
    /// <summary>
    /// Utils for hashes
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Transforms a bytearray to hex string
        /// </summary>
        /// <param name="bytes">bytearray</param>
        /// <param name="upperCase">boolean true if hex should be uppercase</param>
        /// <returns></returns>
        public static string ToHex(byte[] bytes, bool upperCase)
        {
            var result = new StringBuilder(bytes.Length * 2);
            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }
    }
}
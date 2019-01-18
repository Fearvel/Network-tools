using System.Security.Cryptography;
using System.Text;


namespace de.fearvel.net.Security.Hash
{
    public static class Sha256
    {
        public static string GenerateSha256(string StringIn, bool upperCase = true)
        {
            string hashString;
            using (var sha256 = SHA256Managed.Create())
            {
                var hash = sha256.ComputeHash(Encoding.Default.GetBytes(StringIn));
                hashString = Utils.ToHex(hash, upperCase);
            }
            return hashString;
        }
    }
}

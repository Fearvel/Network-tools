using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneTokenOffer
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneTokenOffer : JsonSerializable<ManastoneTokenOffer>
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">token</param>
        public ManastoneTokenOffer(string token)
        {
            Token = token;
        }
    }
}
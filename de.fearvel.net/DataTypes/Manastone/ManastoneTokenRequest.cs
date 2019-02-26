using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneTokenRequest
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneTokenRequest : JsonSerializable<ManastoneActivationOffer>
    {
        /// <summary>
        /// ActivationKey
        /// </summary>
        public string ActivationKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activationKey">activationKey</param>
        public ManastoneTokenRequest(string activationKey)
        {
            ActivationKey = activationKey;
        }
    }
}
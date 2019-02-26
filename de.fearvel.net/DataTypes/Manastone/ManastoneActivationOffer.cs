using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneActivationOffer
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneActivationOffer : JsonSerializable<ManastoneActivationOffer>
    {
        /// <summary>
        /// ActivationKey
        /// </summary>
        public string ActivationKey;

        /// <summary>
        /// validUntil
        /// </summary>
        public DateTime ValidUntil;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activationKey">activationKey</param>
        /// <param name="validUntil">validUntil</param>
        public ManastoneActivationOffer(string activationKey, DateTime validUntil)
        {
            ActivationKey = activationKey;
            ValidUntil = validUntil;
        }
    }
}
using System;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneLicenseInformationOffer
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneLicenseInformationOffer
    {
        /// <summary>
        /// LicenseKey
        /// </summary>
        public string LicenseKey;

        /// <summary>
        /// ValidUntil
        /// </summary>
        public DateTime ValidUntil;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="licenseKey">licenseKey</param>
        /// <param name="dataOfExpiry">dataOfExpiry</param>
        public ManastoneLicenseInformationOffer(string licenseKey, DateTime dataOfExpiry)
        {
            LicenseKey = licenseKey;
            ValidUntil = dataOfExpiry;
        }
    }
}
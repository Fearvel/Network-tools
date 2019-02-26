namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneLicenseInformationRequest
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneLicenseInformationRequest
    {
        /// <summary>
        /// LicenseKey
        /// </summary>
        public string LicenseKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="licenseKey">licenseKey</param>
        public ManastoneLicenseInformationRequest(string licenseKey)
        {
            LicenseKey = licenseKey;
        }
    }
}
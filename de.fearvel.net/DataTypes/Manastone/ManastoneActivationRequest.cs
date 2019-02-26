using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Manastone;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneActivationRequest
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneActivationRequest : JsonSerializable<ManastoneActivationRequest>
    {
        /// <summary>
        /// LicenseKey
        /// </summary>
        public string LicenseKey;

        /// <summary>
        /// HardwareIdentification
        /// </summary>
        public string HardwareIdentification;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="licenseKey">licenseKey</param>
        public ManastoneActivationRequest(string licenseKey)
        {
            LicenseKey = licenseKey;
            HardwareIdentification = ManastoneTools.GetHardwareId();
        }
    }
}
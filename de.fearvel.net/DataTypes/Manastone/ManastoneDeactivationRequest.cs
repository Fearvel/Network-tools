using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Manastone;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneDeactivationRequest
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneDeactivationRequest : JsonSerializable<ManastoneDeactivationRequest> //experimental do not use
    {
        /// <summary>
        /// LicenseKey
        /// </summary>
        public string LicenseKey;

        /// <summary>
        /// ActivationKey
        /// </summary>
        public string ActivationKey;

        /// <summary>
        /// HardwareIdentification
        /// </summary>
        public string HardwareIdentification;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="licenseKey">licenseKey</param>
        /// <param name="activationKey">activationKey</param>
        /// <param name="hardwareId">hardwareId</param>
        public ManastoneDeactivationRequest(string licenseKey, string activationKey, string hardwareId)
        {
            LicenseKey = licenseKey;
            ActivationKey = activationKey;
            HardwareIdentification = ManastoneTools.GetHardwareId();
        }
    }
}
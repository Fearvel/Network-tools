using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Manastone;

namespace de.fearvel.net.DataTypes.Manastone
{
    public class ManastoneDeactivationRequest : JsonSerializable<ManastoneDeactivationRequest>//experimental do not use
    {
        public string LicenseKey;
        public string ActivationKey;
        public string HardwareIdentification;
        public ManastoneDeactivationRequest(string licenseKey, string activationKey, string hardwareId)
        {
            LicenseKey = licenseKey;
            ActivationKey = activationKey;
            HardwareIdentification = ManastoneTools.GetHardwareId();
        }
    }



}


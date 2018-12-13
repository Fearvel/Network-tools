using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Manastone;
namespace de.fearvel.net.DataTypes.Manastone
{
    public class ManastoneActivationRequest : JsonSerializable<ManastoneActivationRequest>
    {
        public string LicenseKey;
        public string HardwareIdentification;

        public ManastoneActivationRequest(string licenseKey)
        {
            LicenseKey = licenseKey;
            HardwareIdentification = ManastoneTools.GetHardwareId();
        }
    }

}


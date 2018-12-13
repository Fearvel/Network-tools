using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net.DataTypes.Manastone
{
    public class ManastoneLicenseInformationRequest
    {
        public string LicenseKey;
        public ManastoneLicenseInformationRequest(string licenseKey)
        {
            LicenseKey = licenseKey;
        }
    }
}

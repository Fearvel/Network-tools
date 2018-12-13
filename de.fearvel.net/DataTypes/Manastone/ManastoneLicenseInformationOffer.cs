using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net.DataTypes.Manastone
{
    public class ManastoneLicenseInformationOffer
    {
        public string LicenseKey;
        public DateTime ValidUntil;
        public ManastoneLicenseInformationOffer(string licenseKey, DateTime dataOfExpiry)
        {
            LicenseKey = licenseKey;
            ValidUntil = dataOfExpiry;
        }
    }
}

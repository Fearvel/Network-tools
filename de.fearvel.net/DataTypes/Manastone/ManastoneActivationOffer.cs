using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{

    public class ManastoneActivationOffer : JsonSerializable<ManastoneActivationOffer>
    {
        public string ActivationKey;
        public DateTime ValidUntil;

        public ManastoneActivationOffer(string activationKey, DateTime validUntil)
        {
            ActivationKey = activationKey;
            ValidUntil = validUntil;
        }
    }

}

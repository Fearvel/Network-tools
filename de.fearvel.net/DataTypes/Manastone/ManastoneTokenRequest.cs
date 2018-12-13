using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{

        public class ManastoneTokenRequest : JsonSerializable<ManastoneActivationOffer>
        {
            public string ActivationKey;
            public ManastoneTokenRequest(string activationKey)
            {
                ActivationKey = activationKey;
            }
        }

    
    
}

using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{

    public class ManastoneTokenOffer : JsonSerializable<ManastoneTokenOffer>
    {
        public string Token;
        public ManastoneTokenOffer(string token)
        {
            Token = token;
        }
    }


}

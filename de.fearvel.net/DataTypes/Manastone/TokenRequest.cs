using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    public sealed class TokenRequest : JsonSerializable<TokenRequest>
    {
        public string ActivationKey;

        public TokenRequest(string activationKey)
        {
            ActivationKey = activationKey;
        }
    }
}
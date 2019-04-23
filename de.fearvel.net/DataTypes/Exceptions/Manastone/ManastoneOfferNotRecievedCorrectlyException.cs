using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net.DataTypes.Exceptions.Manastone
{
    public class ManastoneOfferNotRecievedCorrectlyException : ManastoneException

    {
        public ManastoneOfferNotRecievedCorrectlyException()
        {
        }

        public ManastoneOfferNotRecievedCorrectlyException(string message) : base(message)
        {
        }
    }
}

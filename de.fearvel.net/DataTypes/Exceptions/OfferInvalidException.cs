using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for Invalid Offers
    /// </summary>
    class OfferInvalidException : Exception
    {
        /// <summary>
        /// string message -> error message
        /// int code -> error code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public OfferInvalidException(string message, int code) : base(message + " _ " + code)
        {
            if (FnLog.FnLog.IsInitialized)
            {
                FnLog.FnLog.GetInstance().Log(FnLog.FnLog.LogType.Error, "OfferInvalidException",
                    message + " _ " + code);
            }
        }
    }
}

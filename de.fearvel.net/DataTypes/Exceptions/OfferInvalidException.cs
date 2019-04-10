using System;
using FnLogController = de.fearvel.net.FnLog.FnLog;
namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for Invalid Offers
    /// <copyright>Andreas Schreiner 2019</copyright>
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
            if (FnLogController.IsInitialized)
            {
                FnLogController.GetInstance().Log(FnLogController.LogType.Error, "OfferInvalidException",
                    message + " _ " + code);
            }
        }
    }
}

using System;
namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for declined Messages
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    class RequestDeclinedException : Exception
    {
        /// <summary>
        /// string message -> error message
        /// int code -> error code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public RequestDeclinedException(string message, int code) : base(message + " _ " + code)
        {
            if (FnLog.FnLog.IsInitialized)
            {
                FnLog.FnLog.GetInstance().Log(FnLog.FnLog.LogType.Error, "RequestDeclinedException",
                    message + " _ " + code);
            }
        }
    }
}
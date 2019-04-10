using System;
using FnLogController = de.fearvel.net.FnLog.FnLog;

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
            if (FnLogController.IsInitialized)
            {
                FnLogController.GetInstance().Log(FnLogController.LogType.Error, "RequestDeclinedException",
                    message + " _ " + code);
            }
        }
    }
}
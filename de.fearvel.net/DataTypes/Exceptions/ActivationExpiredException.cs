using System;
using FnLogController = de.fearvel.net.FnLog.FnLog;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for Expired Activations
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    class ActivationExpiredException : Exception
    {
        /// <summary>
        /// string message -> error message
        /// int code -> error code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public ActivationExpiredException(string activationKey) : base(activationKey)
        {
            if (FnLogController.IsInitialized)
            {
                FnLogController.GetInstance().Log(FnLogController.LogType.Error, "ActivationExpiredException",
                    activationKey);
            }
        }
    }
}

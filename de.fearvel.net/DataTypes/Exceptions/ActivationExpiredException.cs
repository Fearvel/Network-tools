using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (FnLog.FnLog.IsInitialized)
            {
                FnLog.FnLog.GetInstance().Log(FnLog.FnLog.LogType.Error, "ActivationExpiredException",
                    activationKey);
            }
        }
    }
}

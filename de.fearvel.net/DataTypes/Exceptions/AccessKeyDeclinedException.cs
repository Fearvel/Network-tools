using System;
namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for the case that the Server declines an Access key
    /// </summary>
    public class AccessKeyDeclinedException : Exception
    {
        public AccessKeyDeclinedException() { }

        /// <summary>
        /// string s -> error message
        /// </summary>
        /// <param name="s"></param>
        public AccessKeyDeclinedException(string s) : base(s) { }
    }
}
using System;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for missing LicenseKeys
    /// </summary>
    class NoLicenceKeyRegistredException : Exception
    {
        public NoLicenceKeyRegistredException()
        {
        }

        /// <summary>
        /// string s -> error message
        /// </summary>
        /// <param name="s"></param>
        public NoLicenceKeyRegistredException(string s) : base(s)
        {
        }
    }
}
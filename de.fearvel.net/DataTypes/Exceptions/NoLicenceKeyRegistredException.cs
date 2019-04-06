using System;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for missing LicenseKeys
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    class NoLicenceKeyRegistredException : Exception
    {
        public NoLicenceKeyRegistredException(){}

        /// <summary>
        /// string s -> error message
        /// </summary>
        /// <param name="s"></param>
        public NoLicenceKeyRegistredException(string s) : base(s)
        {
        }
    }
}
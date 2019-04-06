using System;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for Singletons that needs to be setup before using them
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class InstanceNotSetException : Exception
    {
        public InstanceNotSetException(){}

        /// <summary>
        /// string s -> error message
        /// </summary>
        /// <param name="s"></param>
        public InstanceNotSetException(string s) : base(s){}
    }
}
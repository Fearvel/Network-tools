using System;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for the case that a Result object is null
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ResultNullOrNotReceivedException : Exception
    {
        public ResultNullOrNotReceivedException() { }
    
        /// <summary>
        /// string s -> error message
        /// </summary>
        /// <param name="s"></param>
        public ResultNullOrNotReceivedException(string s) : base(s) { }
    }
}
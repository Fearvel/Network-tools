using System;

namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for failed Queries
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class QueryFailedException : Exception
    {
        /// <summary>
        /// Standard Constructor for the QueryFailedException Class
        /// </summary>
        public QueryFailedException() : base()
        {
        }

        /// <summary>
        /// string message -> error message
        /// </summary>
        /// <param name="message"></param>
        public QueryFailedException(string message) : base(message)
        {
        }
    }
}
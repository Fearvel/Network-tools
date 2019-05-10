using System;

namespace de.fearvel.net.DataTypes.Exceptions.Manastone
{
    /// <summary>
    /// Manastone Abstract Exception Class
    /// </summary>
    public abstract class ManastoneException : Exception
    {
        /// <summary>
        /// constructor
        /// </summary>
        protected ManastoneException() : base()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        protected ManastoneException(string message) : base(message)
        {
        }
    }
}
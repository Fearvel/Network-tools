using System;

namespace de.fearvel.net.DataTypes.Exceptions.Manastone
{
    /// <summary>
    /// Manastone Abstract Exception Class
    /// </summary>
    public abstract class ManastoneException : Exception
    {
        protected ManastoneException() : base()
        {
        }

        protected ManastoneException(string message) : base(message)
        {
        }
    }
}
namespace de.fearvel.net.DataTypes.Exceptions.Manastone
{
    /// <summary>
    /// Exception for the case that an invalid Token offer has been received
    /// </summary>
    class FailedToRetrieveTokenException : ManastoneException
    {
        /// <summary>
        /// constructor
        /// </summary>
        public FailedToRetrieveTokenException()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public FailedToRetrieveTokenException(string message) : base(message)
        {
        }
    }
}

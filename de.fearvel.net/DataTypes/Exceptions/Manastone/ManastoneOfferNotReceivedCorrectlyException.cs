namespace de.fearvel.net.DataTypes.Exceptions.Manastone

{
    /// <summary>
    /// Exception for the case that an invalid offer has been received
    /// </summary>
    public class ManastoneOfferNotReceivedCorrectlyException : ManastoneException
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ManastoneOfferNotReceivedCorrectlyException()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ManastoneOfferNotReceivedCorrectlyException(string message) : base(message)
        {
        }
    }
}

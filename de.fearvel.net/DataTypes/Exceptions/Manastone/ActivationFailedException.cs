namespace de.fearvel.net.DataTypes.Exceptions.Manastone
{

    /// <summary>
    /// Exception for failed activations
    /// </summary>
    public class ActivationFailedException : ManastoneException
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ActivationFailedException() : base()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ActivationFailedException(string message) : base(message)
        {
        }
    }
}

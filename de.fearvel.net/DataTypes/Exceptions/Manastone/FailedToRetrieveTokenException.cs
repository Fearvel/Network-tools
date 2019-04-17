namespace de.fearvel.net.DataTypes.Exceptions.Manastone
{
    class FailedToRetrieveTokenException: ManastoneException

    {
        public FailedToRetrieveTokenException()
        {
        }

        public FailedToRetrieveTokenException(string message) : base(message)
        {
        }
    }
}

using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ActivationOnlineCheckOffer Class, Wraps an ActivationStatus.
    /// Is used for Serialized transmission of data
    /// </summary>
    public sealed class ActivationOnlineCheckOffer :JsonSerializable<ActivationOnlineCheckOffer>
    {
        /// <summary>
        /// Contains the ActivationKey
        /// </summary>
        private long _activationStatus;

        /// <summary>
        /// Flag which determines if an _activationStatus has been written
        /// </summary
        private bool _activationStatusSet;

        /// <summary>
        /// Property which gets or sets the _activationStatus
        /// Workaround to make _activationStatus one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public long ActivationStatus
        {
            get => _activationStatus;
            set
            {
                if (!_activationStatusSet)
                {
                    _activationStatus = value;
                    _activationStatusSet = true;
                }
            }
        }

        /// <summary>
        /// Returns a bool
        /// true == 1
        /// false != 1
        /// </summary>
        public bool IsActivated => _activationStatus == 1;
    }
}

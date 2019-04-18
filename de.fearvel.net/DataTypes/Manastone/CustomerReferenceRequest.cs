using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    public sealed class CustomerReferenceRequest : JsonSerializable<ActivationOnlineCheckOffer>
    {
        /// <summary>
        /// Contains the ActivationKey
        /// </summary>
        private string _activationKey;

        /// <summary>
        /// Flag which determines if an _activationKey has been written
        /// </summary>
        private bool _activationKeySet;

        /// <summary>
        /// Property which gets or sets the _activationKey
        /// Workaround to make _activationKey one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public string ActivationKey
        {
            get => _activationKey;
            set
            {
                if (!_activationKeySet)
                {
                    _activationKey = value;
                    _activationKeySet = true;
                }
            }
        }

        /// <summary>
        /// Constructor of CustomerReferenceRequest
        /// </summary>
        /// <param name="activationKey"></param>
        public CustomerReferenceRequest(string activationKey)
        {
            ActivationKey = activationKey;
        }
    }
}
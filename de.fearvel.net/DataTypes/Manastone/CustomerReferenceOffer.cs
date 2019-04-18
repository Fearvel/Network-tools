using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    public sealed class CustomerReferenceOffer : JsonSerializable<ActivationOnlineCheckOffer>
    {

        /// <summary>
        /// Contains the CustomerReference
        /// </summary>
        private string _customerReference;

        /// <summary>
        /// Flag which determines if an _customerReference has been written
        /// </summary>
        private bool _customerReferenceSet;


        /// <summary>
        /// Property which gets or sets the _customerReference
        /// Workaround to make _activationKey one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public string CustomerReference
        {
            get => _customerReference;
            set
            {
                if (!_customerReferenceSet)
                {
                    _customerReference = value;
                    _customerReferenceSet = true;
                }
            }
        }
    }
}
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    public sealed class CheckTokenOffer : JsonSerializable<CheckTokenOffer>
    {
        /// <summary>
        /// Contains the TokenCheck value
        /// </summary>
        private long _tokenCheck;

        /// <summary>
        /// Flag which determines if an _tokenCheck has been written
        /// </summary>
        private bool _tokenCheckSet;

        /// <summary>
        /// Property which gets or sets the _tokenCheck
        /// Workaround to make _activationKey one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public long TokenCheck
        {
            get => _tokenCheck;
            set
            {
                if (!_tokenCheckSet)
                {
                    _tokenCheck = value;
                    _tokenCheckSet = true;
                }
            }
        }

        /// <summary>
        /// Returns a bool
        /// true == 1
        /// false != 1
        /// </summary>
        public bool IsValid=> _tokenCheck == 1;
    }
}
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    public sealed class CheckTokenRequest : JsonSerializable<CheckTokenRequest>
    {
        /// <summary>
        /// Contains the Token
        /// </summary>
        private string _token;

        /// <summary>
        /// Flag which determines if an _token has been written
        /// </summary>
        private bool _tokenSet;

        /// <summary>
        /// Property which gets or sets the _token
        /// Workaround to make _activationStatus one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public string Token
        {
            get => _token;
            set
            {
                if (!_tokenSet)
                {
                    _token = value;
                    _tokenSet = true;
                }
            }
        }

        /// <summary>
        /// Constructor of CheckTokenRequest
        /// </summary>
        /// <param name="token"></param>
        public CheckTokenRequest(string token)
        {
            Token = token;
        }
    }
}
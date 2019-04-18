using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Security;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ActivationOffer Class, Wraps an ActivationKey and HardwareId.
    /// Is used for Serialized transmission of data
    /// </summary>
    public sealed class ActivationOnlineCheckRequest : JsonSerializable<ActivationOnlineCheckRequest>
    {
        /// <summary>
        /// Contains the ActivationKey
        /// </summary>
        private string _activationKey;

        /// <summary>
        /// Contains the HardwareId
        /// </summary>
        private string _hardwareId;

        /// <summary>
        /// Flag which determines if an _activationKey has been written
        /// </summary>
        private bool _activationKeySet;

        /// <summary>
        /// Flag which determines if an _hardwareId has been written
        /// </summary>
        private bool _hardwareIdSet;

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
        /// Property which gets or sets the _hardwareId
        /// Workaround to make _hardwareId one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public string HardwareId
        {
            get => _hardwareId;
            set
            {
                if (!_hardwareIdSet)
                {
                    _hardwareId = value;
                    _hardwareIdSet = true;
                }
            }
        }

        /// <summary>
        /// Constructor of ActivationOnlineCheckRequest
        /// </summary>
        /// <param name="activationKey"></param>
        public ActivationOnlineCheckRequest(string activationKey)
        {
            ActivationKey = activationKey;
            HardwareId = Ident.GetCPUId();
        }
    }
}
using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Security;

namespace de.fearvel.net.DataTypes.Manastone
{
    public sealed class ActivationRequest : JsonSerializable<ActivationRequest>
    {
        /// <summary>
        /// Contains the SerialNumber
        /// </summary>
        private string _serialNumber;

        /// <summary>
        /// Contains the HardwareId
        /// </summary>
        private string _hardwareId;

        /// <summary>
        /// Contains the 
        /// </summary>
        private string _productUuid;

        /// <summary>
        /// Flag which determines if an _serialNumber has been written
        /// </summary>
        private bool _serialNumberSet;

        /// <summary>
        /// Flag which determines if an _hardwareId has been written
        /// </summary>
        private bool _hardwareIdSet;

        /// <summary>
        /// Flag which determines if an _productUuid has been written
        /// </summary>
        private bool _productUuidSet;

        /// <summary>
        /// Property which gets or sets the _serialNumber
        /// Workaround to make _activationKey one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                if (!_serialNumberSet)
                {
                    _serialNumber = value;
                    _serialNumberSet = true;
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
        /// Property which gets or sets the _productUuid
        /// Workaround to make _hardwareId one time writable
        /// This workaround is needed because the JSON Deserializer
        /// can only handle a public setter 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string ProductUUID
        {
            get => _productUuid;
            set
            {
                if (!_productUuidSet)
                {
                    _productUuid = value;
                    _productUuidSet = true;
                }
            }
        }

        /// <summary>
        /// Constructor of ActivationRequest
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="productUuid"></param>
        public ActivationRequest(string serialNumber, string productUuid)
        {
            SerialNumber = serialNumber;
            HardwareId = Ident.GetCPUId();
            ProductUUID = productUuid;
        }
    }
}
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.Exceptions.Manastone;
using de.fearvel.net.DataTypes.Manastone;
using de.fearvel.net.SocketIo;
using System;
using System.Runtime.CompilerServices;

namespace de.fearvel.net.Manastone
{
    /// <summary>
    /// Manastone DRM Client Class
    /// </summary>
    public sealed class ManastoneClient
    {
        #region "Private"

        /// <summary>
        /// Url of the Manastone Server
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// Instance of ManastoneDatabase
        /// </summary>
        private readonly ManastoneDatabase _database;

        /// <summary>
        /// The uuid of the Program, which will be checked
        /// </summary>
        private readonly string _productUuid;

        /// <summary>
        /// Enum which determines if:
        /// The Program will be checked for Activation Online, Offline or Periodically online(Mixed)
        /// </summary>
        private readonly LicenseCheckType _licCheck;


        /// <summary>
        /// the Instance of this Singleton
        /// </summary>
        private static ManastoneClient _instance;


        /// <summary>
        /// Private Constructor
        /// </summary>
        /// <param name="serverUrl"></param>
        private ManastoneClient(string serverUrl, string productUUID, LicenseCheckType lCheck)
        {
            _url = serverUrl;
            _database = new ManastoneDatabase();
            _productUuid = productUUID;
            _licCheck = lCheck;
            ManastoneServerVersion = RetrieveManastoneServerVersion().ToString();
        }

        /// <summary>
        /// Returns the Version of the Manastone Server
        /// </summary>
        /// <returns></returns>
        private Version RetrieveManastoneServerVersion()
        {
            return Version.Parse(SocketIoClient.RetrieveSingleValue<VersionWrapper>(_url, "ManastoneVersion",
                "ManastoneVersionRequest", null).Version);
        }

        /// <summary>
        /// Returns the DateTime of the Manastone Server
        /// Currently unused, will be used later for sure
        /// </summary>
        /// <returns></returns>
        private DateTime RetrieveServerTime()
        {
            return SocketIoClient.RetrieveSingleValue<DateTimeWrapper>(_url, "ServerTime", "ServerTimeRequest", null)
                .Time;
        }

        /// <summary>
        /// Activates a License
        /// The License can only be Activated once at the same Time for each SerialNumber 
        /// </summary>
        /// <param name="req"></param>
        private void Activate(ActivationRequest req)
        {
            var activationKey =
                SocketIoClient.RetrieveSingleValue<ActivationOffer>(_url, "ActivationOffer", "ActivationRequest",
                    req.Serialize());
            if (activationKey.ActivationKey.Length == 0)
                throw new ActivationFailedException();

            _database.InsertSerialNumber(req.SerialNumber);
            _database.InsertActivationKey(req.SerialNumber, activationKey.ActivationKey);
            RetrieveToken(new TokenRequest(activationKey.ActivationKey));
            RetrieveCustomerReference(new CustomerReferenceRequest(activationKey.ActivationKey));
        }

        /// <summary>
        /// Retrieves the Customer Reference and saves it to the Local database
        /// </summary>
        /// <param name="req"></param>
        private void RetrieveCustomerReference(CustomerReferenceRequest req )
        {
            var offer =
                SocketIoClient.RetrieveSingleValue<CustomerReferenceOffer>(_url, "CustomerReferenceOffer",
                    "CustomerReferenceRequest", req.Serialize());
            _database.InsertCustomerReference(req.ActivationKey, offer.CustomerReference);
        }

        /// <summary>
        /// Retrieves the Token and saves it to the Local database
        /// </summary>
        /// <param name="req"></param>
        private void RetrieveToken(TokenRequest req)
        {
            var token =
                SocketIoClient.RetrieveSingleValue<TokenOffer>(_url, "TokenOffer", "TokenRequest",
                    req.Serialize());
            if (token.Token.Length == 0)
                throw new FailedToRetrieveTokenException();
            _database.InsertToken(req.ActivationKey, token.Token);
        }

        /// <summary>
        /// Checks if an entry of an Activation is written to the local encrypted
        /// Manastone database
        /// </summary>
        /// <returns></returns>
        private bool CheckLicenseStatusLocally()
        {
            return _database.LicenseInstalled();
        }

        /// <summary>
        /// Checks Activation status.
        /// if valid returns true
        /// </summary>
        /// <param name="req">ActivationOnlineCheckRequest</param>
        /// <returns></returns>
        private bool CheckLicenseStatusOnline(ActivationOnlineCheckRequest req)
        {
            try
            {
                var offer = SocketIoClient.RetrieveSingleValue<ActivationOnlineCheckOffer>(_url, "ActivationOnlineCheckOffer",
                            "ActivationOnlineCheckRequest", req.Serialize());
                if (offer.IsActivated)
                {
                    RetrieveCustomerReference(new CustomerReferenceRequest(req.ActivationKey));
                }
                return offer.IsActivated;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the Token is valid
        /// </summary>
        /// <param name="req">CheckTokenRequest</param>
        /// <returns></returns>
        public bool CheckToken(CheckTokenRequest req)
        {
            var offer = SocketIoClient.RetrieveSingleValue<CheckTokenOffer>(_url, "CheckTokenOffer",
                "CheckTokenRequest", req.Serialize());
            return offer.IsValid;
        }
        #endregion

        #region "Public"

        /// <summary>
        /// The Version of this Manastone Client Class
        /// Can be used later to determine if this client is outdated 
        /// </summary>
        public string ClientVersion => "1.0.0.0";

        /// <summary>
        /// CustomerReference Property
        /// Get only
        /// Gets the CustomerReference for the activated SerialNumber
        /// </summary>
        public string CustomerReference => _database.CustomerReference;

        /// <summary>
        /// The Version of this Manastone Server 
        /// Can be used later to determine if this client is outdated
        /// </summary>
        public string ManastoneServerVersion;

        /// <summary>
        /// GetInstance for the Singleton
        /// </summary>
        /// <returns>instance</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ManastoneClient GetInstance()
        {
            return _instance ?? throw new InstanceNotSetException();
        }

        /// <summary>
        /// Sets the Instance of ManastoneClient
        /// Used to preset values like the Server URL
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="productUuid"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetInstance(string serverUrl, string productUuid,
            LicenseCheckType licCheck = LicenseCheckType.Online)
        {
            _instance = new ManastoneClient(serverUrl, productUuid, licCheck);
        }

        /// <summary>
        /// Activates a License
        /// The License can only be Activated once at the same Time for each SerialNumber 
        /// </summary>
        /// <returns></returns>
        public bool Activate(string serialNumber)
        {
            try
            {
                Activate(new ActivationRequest(serialNumber, _productUuid));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the ActivationKey
        /// The LicenseCheckType determines if this will be
        /// online only, offline only, or mixed
        /// Mixed 1 in 7 chance to check online
        /// </summary>
        /// <returns></returns>
        public bool CheckActivation()
        {
            try
            {
                switch (_licCheck)
                {
                    case LicenseCheckType.Online:
                        return CheckLicenseStatusOnline(new ActivationOnlineCheckRequest(_database.ActivationKey));
                    case LicenseCheckType.Offline:
                        return CheckLicenseStatusLocally();
                    case LicenseCheckType.Mixed:
                        var rand = new Random();
                        return rand.Next(100) % 7 == 0 ?
                            CheckLicenseStatusOnline(new ActivationOnlineCheckRequest(_database.ActivationKey))
                            : CheckLicenseStatusLocally();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the Token is valid
        /// </summary>
        /// <returns></returns>
        public bool CheckToken()
        {
            try
            {
                return CheckToken(new CheckTokenRequest(_database.Token));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Enum for the ActivationCheck type that will be used
        /// </summary>
        public enum LicenseCheckType
        {
            Online,
            Offline,
            Mixed
        }

        #endregion
    }
}
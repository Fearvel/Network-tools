using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.Exceptions.Manastone;
using de.fearvel.net.DataTypes.Manastone;
using de.fearvel.net.SocketIo;
using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace de.fearvel.net.Manastone
{
    /// <summary>
    /// Manastone DRM Client Class
    /// used for the communication with the Manastone server
    /// this class will create an instance on ManastoneDatabase
    /// </summary>
    public sealed class ManastoneClient
    {
        #region "Private"

        /// <summary>
        /// Timeout of the SocketIo RetrieveSingleValue function
        /// </summary>
        private readonly int _timeout = 30000;

        /// <summary>
        /// Url of the Manastone Server
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// Instance of ManastoneDatabase, which will be used for the local storage of the License information
        /// </summary>
        private readonly ManastoneDatabase _database;

        /// <summary>
        /// The uuid of the Program, which will be checked
        /// Every program which witch uses Manastone DRM needs to have a UUID that is known by the Manastone Server 
        /// </summary>
        private readonly string _productUuid;

        /// <summary>
        /// Enum which determines if:
        /// The Program will be checked for Activation Online, Offline or Periodically online(Mixed)
        /// This is for changing the Telemetry mode of the FnLog Telemetry system
        /// </summary>
        private readonly LicenseCheckType _licCheck;


        /// <summary>
        /// the Instance of this Singleton
        /// There can only be one instance of this
        /// </summary>
        private static ManastoneClient _instance;


        /// <summary>
        /// Property for accessing the Manastone log by Programs that implement Manastone DRM
        /// </summary>
        public DataTable ManastoneLog => _database.Log.GetLog();

        /// <summary>
        /// Private Constructor to ensure that there will be only one instance
        /// this will initialize properties
        /// creates the ManastoneDatabase instance
        /// It will also retrieve the Manastone serverVersion
        /// and try to Receive the CustomerReference 
        /// </summary>
        /// <param name="serverUrl"></param>
        private ManastoneClient(string serverUrl, string productUUID, LicenseCheckType lCheck)
        {
            _url = serverUrl;
            _database = new ManastoneDatabase();
            _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmLog, "MANASTONE STARTED", "");
            _productUuid = productUUID;
            _licCheck = lCheck;
            ManastoneServerVersion = RetrieveManastoneServerVersion().ToString();
            RetrieveCustomerReference(new CustomerReferenceRequest(_database.ActivationKey));
        }

        /// <summary>
        /// Returns the Version of the Manastone Server
        /// if this fails it will throw a ManastoneOfferNotReceivedCorrectlyException
        /// </summary>
        /// <returns></returns>
        private Version RetrieveManastoneServerVersion()
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MinorDrmLog, "RetrieveManastoneServerVersion", "Start");
                var serVer = Version.Parse(SocketIoClient.RetrieveSingleValue<VersionWrapper>(_url, "ManastoneVersion",
                    "ManastoneVersionRequest", null, timeout: _timeout).Version);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MinorDrmLog, "RetrieveManastoneServerVersion", "Complete");
                _database.Log.ProcessLogList();
                return serVer;
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on RetrieveManastoneServerVersion", e.Message);
                _database.Log.ProcessLogList();
                throw new ManastoneOfferNotReceivedCorrectlyException();
            }
        }

        /// <summary>
        /// Returns the DateTime of the Manastone Server
        /// Currently unused, will be used later to locally check the Token expiry date
        /// </summary>
        /// <returns></returns>
        private DateTime RetrieveServerTime()
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MinorDrmLog, "RetrieveServerTime", "Start");
                var serTime = SocketIoClient
                    .RetrieveSingleValue<DateTimeWrapper>(_url, "ServerTime", "ServerTimeRequest", null)
                    .Time;
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MinorDrmLog, "RetrieveServerTime", "Complete");
                _database.Log.ProcessLogList();
                return serTime;
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on RetrieveServerTime", e.Message);
                _database.Log.ProcessLogList();
                throw new ManastoneOfferNotReceivedCorrectlyException();
            }
        }

        /// <summary>
        /// Activates a License
        /// the License must be valid and non activated
        /// there can only be one activation of a SerialNumber
        /// If the Activation is successful the SerialNumber and ActivationKey will be inserted into the local database.
        /// It will throw a ActivationFailedException if the ManastoneServer delivers no or an empty Offer.
        /// </summary>
        /// <param name="req">an ActivationRequest containing the SerialNumber and the CPU1 ID</param>
        private void Activate(ActivationRequest req)
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "req " + req.SerialNumber + " " + req.ProductUUID + " " + req.HardwareId); //DEBUG

                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "Start");
                var activationKey =
                    SocketIoClient.RetrieveSingleValue<ActivationOffer>(_url, "ActivationOffer", "ActivationRequest",
                        req.Serialize(), timeout: _timeout);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "ActivationOffer " + activationKey.ActivationKey.Length);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "ActivationOffer " + activationKey.Serialize());

                if (activationKey.ActivationKey.Length == 0)
                {
                    throw new ActivationFailedException("Length == 0");
                }
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "Before SerialInsertion");
                _database.InsertSerialNumber(req.SerialNumber);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "Before ActivationKeyInsertion");
                _database.InsertActivationKey(req.SerialNumber, activationKey.ActivationKey);
                RetrieveToken(new TokenRequest(activationKey.ActivationKey));
                RetrieveCustomerReference(new CustomerReferenceRequest(activationKey.ActivationKey));
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "Activate", "Complete");
                _database.Log.ProcessLogList();
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on ACTIVATE", e.Message);
                _database.Log.ProcessLogList();
                throw new ActivationFailedException();
            }
        }

        /// <summary>
        /// Retrieves the Customer Reference from the Manastone Server and saves it to the Local database
        /// </summary>
        /// <param name="req">an CustomerReferenceRequest containing the ActivationKey</param>
        private void RetrieveCustomerReference(CustomerReferenceRequest req)
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "RetrieveCustomerReference", "Start");
                var offer =
                    SocketIoClient.RetrieveSingleValue<CustomerReferenceOffer>(_url, "CustomerReferenceOffer",
                        "CustomerReferenceRequest", req.Serialize(), timeout: _timeout);
                _database.InsertCustomerReference(req.ActivationKey, offer.CustomerReference);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "RetrieveCustomerReference", "Complete");
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on RetrieveCustomerReference", e.Message);
                throw new ManastoneOfferNotReceivedCorrectlyException();
            }
        }

        /// <summary>
        /// Retrieves the Token and saves it to the Local database
        /// </summary>
        /// <param name="req">an TokenRequest containing the ActivationKey</param>
        private void RetrieveToken(TokenRequest req)
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "RetrieveToken", "Start");
                var token =
                    SocketIoClient.RetrieveSingleValue<TokenOffer>(_url, "TokenOffer", "TokenRequest",
                        req.Serialize(), timeout: _timeout);
                if (token.Token.Length == 0)
                    throw new FailedToRetrieveTokenException();
                _database.InsertToken(req.ActivationKey, token.Token);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "RetrieveToken", "Complete");
                _database.Log.ProcessLogList();
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on RetrieveToken", e.Message);
                _database.Log.ProcessLogList();
                throw new ManastoneOfferNotReceivedCorrectlyException();
            }
        }

        /// <summary>
        /// Checks if an entry of an Activation is written to the local encrypted Manastone database
        /// </summary>
        /// <returns>a bool that is true if the license is valid</returns>
        private bool CheckLicenseStatusLocally()
        {
            var status= _database.LicenseInstalled();
            _database.Log.AddToLogList(FnLog.FnLog.LogType.MinorDrmLog, "CheckLicenseStatusLocally", status.ToString());
            _database.Log.ProcessLogList();
            return status;
        }

        /// <summary>
        /// Checks Activation status Online.
        /// this will contact the Manastone server and check if the activation is valid
        /// </summary>
        /// <param name="req">ActivationOnlineCheckRequest</param>
        /// <returns>if valid returns true</returns>
        private bool CheckLicenseStatusOnline(ActivationOnlineCheckRequest req)
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "CheckLicenseStatusOnline", "Start");
                var offer = SocketIoClient.RetrieveSingleValue<ActivationOnlineCheckOffer>(_url,
                    "ActivationOnlineCheckOffer",
                    "ActivationOnlineCheckRequest", req.Serialize(), timeout: _timeout);
                if (offer.IsActivated)
                {
                    _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "CheckLicenseStatusOnline", "ACTIVATION Confirmed");
                    RetrieveCustomerReference(new CustomerReferenceRequest(req.ActivationKey));
                }
                _database.Log.AddToLogList(FnLog.FnLog.LogType.MajorDrmLog, "CheckLicenseStatusOnline", "Complete");
                _database.Log.ProcessLogList();
                return offer.IsActivated;
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on CheckLicenseStatusOnline", e.Message);
                _database.Log.ProcessLogList();
                return false;
            }
        }

        /// <summary>
        /// Checks if the Token is valid
        /// this will contact the Manastone server and check if the token is valid
        /// a token check has to be Online only because the token is to authenticate a program client to a program server or service
        /// if the server doesn't answer it will throw a ManastoneOfferNotReceivedCorrectlyException
        /// </summary>
        /// <param name="req">CheckTokenRequest</param>
        /// <returns></returns>
        public bool CheckToken(CheckTokenRequest req)
        {
            try
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmLog, "CheckToken", "Start");
                var offer = SocketIoClient.RetrieveSingleValue<CheckTokenOffer>(_url, "CheckTokenOffer",
                    "CheckTokenRequest", req.Serialize(), timeout: _timeout);
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmLog, "CheckToken", "Complete");
                _database.Log.ProcessLogList();
                return offer.IsValid;
            }
            catch (Exception e)
            {
                _database.Log.AddToLogList(FnLog.FnLog.LogType.DrmError, "ERROR on CheckToken", e.Message);
                _database.Log.ProcessLogList();
                throw new ManastoneOfferNotReceivedCorrectlyException();
            }
        }

        #endregion

        #region "Public"

        /// <summary>
        /// The Version of this Manastone Client Class
        /// Can be used later to determine if this client is outdated 
        /// </summary>
        public static string ClientVersion => "1.000.0001.0000";

        /// <summary>
        /// CustomerReference Property
        /// Get only
        /// Gets the CustomerReference for the activated SerialNumber
        /// accesses a property of the ManastoneDatabase instance _database
        /// </summary>
        public string CustomerReference => _database.CustomerReference;

        /// <summary>
        /// Token Property
        /// Get only
        /// Gets the Token for the activated SerialNumber
        /// accesses a property of the ManastoneDatabase instance _database
        /// </summary>
        public string Token => _database.Token;

        /// <summary>
        /// The Version of this Manastone Server 
        /// Can be used later to determine if this client is outdated
        /// </summary>
        public string ManastoneServerVersion;

        /// <summary>
        /// GetInstance for the Singleton
        /// if the instance is not set throws InstanceNotSetException
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
        /// <param name="serverUrl">the URL of the Manastone Server</param>
        /// <param name="productUuid">the id of the Product to be activated</param>
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
        /// <returns>bool true if activation successful</returns>
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
        /// Mixed 1 in 7 chance to be checked online
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
                        return rand.Next(100) % 7 == 0
                            ? CheckLicenseStatusOnline(new ActivationOnlineCheckRequest(_database.ActivationKey))
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
        /// Online Check
        /// </summary>
        /// <returns></returns>
        public bool CheckToken()
        {
            try
            {
                if (CheckToken(new CheckTokenRequest(_database.Token)))
                {
                    RetrieveToken(new TokenRequest(_database.ActivationKey));
                }

                return true;
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
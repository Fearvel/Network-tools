using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.Exceptions.Manastone;
using de.fearvel.net.DataTypes.Manastone;
using de.fearvel.net.SocketIo;
using System;
using System.Runtime.CompilerServices;

namespace de.fearvel.net.Manastone
{
    public class ManastoneClient
    {
        #region "Private"

        private readonly string _url;
        private readonly ManastoneDatabase _database;
        private readonly string _productUuid;
        private LicenseCheckType _lCheck;


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
            _lCheck = lCheck;
        }

        public Version RetrieveManastoneServerVersion()
        {
            return Version.Parse(SocketIoClient.RetrieveSingleValue<VersionWrapper>(_url, "ManastoneVersion",
                "ManastoneVersionRequest", null).Version);
        }

        public DateTime RetrieveServerTime()
        {
            return SocketIoClient.RetrieveSingleValue<DateTimeWrapper>(_url, "ServerTime", "ServerTimeRequest", null)
                .Time;
        }

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

        }


        public string RetrieveCustomerReference(string activationKey)
        {
            throw new NotImplementedException();
        }


        public void RetrieveToken(TokenRequest req)
        {
            var token =
                SocketIoClient.RetrieveSingleValue<TokenOffer>(_url, "TokenOffer", "TokenRequest",
                    req.Serialize());
            if (token.Token.Length == 0)
                throw new FailedToRetrieveTokenException();
            _database.InsertToken(req.ActivationKey, token.Token);
        }

        public bool CheckLicenseStatusLocally()
        {
            return _database.LicenseInstalled();
        }

        public bool CheckLicenseStatusOnline()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Public"

        public string ClientVersion => "0.0.1.0";


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
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetInstance(string serverUrl, string productUUID,
            LicenseCheckType lCheck = LicenseCheckType.Online)
        {
            _instance = new ManastoneClient(serverUrl, productUUID, lCheck);
        }

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

        public bool CheckActivation()
        {
            switch (_lCheck)
            {
                case LicenseCheckType.Online:
                    return CheckLicenseStatusOnline();
                case LicenseCheckType.Offline:
                    return CheckLicenseStatusLocally();
                case LicenseCheckType.Mixed:
                    var rand = new Random();
                    if (rand.Next(100) % 7 == 1)
                    {
                        return CheckLicenseStatusOnline();
                    }
                    else
                    {
                        return CheckLicenseStatusLocally();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool CheckToken()
        {
            throw new NotImplementedException();


        }


        public enum LicenseCheckType
        {
            Online,
            Offline,
            Mixed
        }

        #endregion
    }
}
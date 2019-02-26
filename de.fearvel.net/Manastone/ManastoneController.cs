using System;
using System.Data;
using System.Data.SQLite;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.SocketIo;
using de.fearvel.net.DataTypes.Manastone;
using de.fearvel.net.DataTypes.SocketIo;
using de.fearvel.net.SQL.Connector;
using de.fearvel.net.Security.Hash;

namespace de.fearvel.net.Manastone
{
    /// <summary>
    /// EXPERIMENTAL but usable
    /// Controller class for the Manastone DRM System
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneController
    {
        /// <summary>
        /// Version of this Manastone Client
        /// </summary>
        public static Version ManastoneClientVersion => Version.Parse("1.0.0.0");

        /// <summary>
        /// ActivationType enum
        /// </summary>
        private enum ActivationType
        {
            Activation,
            Reactivation
        }

        /// <summary>
        /// Initialization vector
        /// </summary>
        private readonly string _initializationVector;

        /// <summary>
        /// Program UUID
        /// </summary>
        private readonly string _programUUID;

        /// <summary>
        /// key for the Manastone database
        /// </summary>
        private readonly string _fileKey;

        /// <summary>
        /// name of the Manastone database
        /// </summary>
        private string FileName => "Manastone.db";

        /// <summary>
        /// Sqlite connection
        /// </summary>
        private SqliteConnector _conn;

        /// <summary>
        /// Expiry date of the Activation
        /// </summary>
        private DateTime? _activationExpiry = null;

        /// <summary>
        /// RO acces of _activationExpiry
        /// </summary>
        public DateTime? ActivationExpiry => _activationExpiry;


        /// <summary>
        /// TESTING
        /// Removes the DB encryption
        /// </summary>
        public void RemoveEncryption()
        {
            _conn.SetPassword("");
        }

        /// <summary>
        /// instance of the singleton
        /// </summary>
        private static ManastoneController _instance;

        /// <summary>
        /// GetInstance for this Singleton
        /// </summary>
        /// <returns>ManastoneController</returns>
        public static ManastoneController GetInstance()
        {
            if (_instance == null)
                throw new InstanceNotSetException();
            return _instance;
        }

        /// <summary>
        /// Setter for the instance
        ///  because initialization vector and programUUID differs from program to program  
        /// </summary>
        /// <param name="initializationVector">initializationVector</param>
        /// <param name="programUUID">programUUID</param>
        // ReSharper disable once InconsistentNaming
        public static void SetInstance(string initializationVector, string programUUID)
        {
            _instance = new ManastoneController(initializationVector, programUUID);
        }

        /// <summary>
        /// Creates the SqliteConnector
        /// </summary>
        private void LoadDatabaseConnection()
        {
            _conn = new SqliteConnector(FileName, _fileKey);
        }

        /// <summary>
        /// Creates the Tables if necessary
        /// </summary>
        private void CreateTables()
        {
            CreateDirectoryTable();
            CreateLicenseKeyTable();
            CreateTokenTable();
            CreateActivationTable();
        }

        /// <summary>
        /// creates the Directory Table
        /// </summary>
        private void CreateDirectoryTable() =>
            _conn.NonQuery(
                "CREATE TABLE IF NOT EXISTS `Directory` (" +
                "`DKey` INTEGER," +
                "`DVal` INTEGER" +
                ");"
            );

        /// <summary>
        /// Creates the Token Table
        /// and a required trigger
        /// </summary>
        private void CreateTokenTable()
        {
            _conn.NonQuery(
                "CREATE TABLE IF NOT EXISTS `Token` ( " +
                "`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "`Token` TEXT, " +
                "`DOE` DateTime " +
                ");"
            );
            _conn.NonQuery(
                "CREATE TRIGGER IF NOT EXISTS RemoveInvalidTokens before INSERT on Token " +
                "BEGIN " +
                "delete from Token where DOE < date(); " +
                "END; "
            );
        }

        /// <summary>
        /// Creates the Activation Table
        /// and a required trigger
        /// </summary>
        private void CreateActivationTable()
        {
            _conn.NonQuery(
                "CREATE TABLE IF NOT EXISTS `Activation` ( " +
                "`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "`ActivationKey` TEXT, " +
                "`DOE` DateTime " +
                ");"
            );
            _conn.NonQuery(
                "CREATE TRIGGER IF NOT EXISTS RemoveInvalidActivations before INSERT on Activation " +
                "BEGIN " +
                "delete from Activation where DOE < date(); " +
                "END; "
            );
        }

        /// <summary>
        /// Creates the LicenseKey Table
        /// and a required trigger
        /// </summary>
        private void CreateLicenseKeyTable()
        {
            _conn.NonQuery(
                "CREATE TABLE IF NOT EXISTS `LicenseKey` ( " +
                "`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "`LicenseKey` TEXT, " +
                "`DOE` DateTime " +
                ");"
            );
            _conn.NonQuery(
                "CREATE TRIGGER IF NOT EXISTS RemoveInvalidLicenseKeys before INSERT on LicenseKey " +
                "BEGIN " +
                "delete from LicenseKey where DOE < date(); " +
                "END; "
            );
        }

        /// <summary>
        /// Purges the Tables from outdated entries
        /// </summary>
        private void AutoClean() =>
            _conn.NonQuery(
                "delete from LicenseKey where DOE < date() and DOE not null; " +
                "delete from Token where DOE < date(); " +
                "delete from Activation where DOE < date(); "
            );

        /// <summary>
        /// Checks if the Activation is valid
        /// </summary>
        /// <returns>bool true == valid</returns>
        public bool CheckActivation()
        {
            AutoClean();
            try
            {
                if ((GetActivationKey().Length <= 0 || !CheckActivation())) return false;
                _activationExpiry = CheckAndReturnActivation().DateOfExpiry;
            }
            catch (ActivationExpiredException)
            {
                Activator(ActivationType.Reactivation);
                return CheckActivation();
            }

            return true;
        }

        /// <summary>
        /// Activates the Program
        /// </summary>
        private void Activate()
        {
            Activator(ActivationType.Activation);
        }

        /// <summary>
        /// Creates an Activation request sends it to the server and waits for respnse
        /// </summary>
        /// <param name="at"></param>
        private void Activator(ActivationType at)
        {
            var licenseKey = GetLicenseKey();
            ClearActivationAndTokens();
            CheckLicenseKey(licenseKey);
            string requestType = "";

            switch (at)
            {
                case ActivationType.Activation:
                    requestType = "ActivationRequest";
                    break;
                case ActivationType.Reactivation:
                    requestType = "ReActivationRequest";
                    break;
                default:
                    throw new Exception();
            }

            var ow =
                SocketIoClient
                    .RetrieveSingleValue<OfferWrapper<ManastoneActivationOffer>>( //Server hardcoded for testing
                        "https://127.0.0.1:9041", "ActivationOffer", requestType,
                        new ManastoneActivationRequest(licenseKey).Serialize());
            if (!ow.Result.Result)
            {
                throw new RequestDeclinedException(ow.Result.Message, ow.Result.Code);
            }

            if (!CheckActivationOffer(ow.Data))
            {
                throw new OfferInvalidException(ow.Result.Message, ow.Result.Code);
            }

            InsertActivationKey(ow.Data.ActivationKey, ow.Data.ValidUntil);
            _activationExpiry = ow.Data.ValidUntil;
        }

        /// <summary>
        /// Removes all activationTokens
        /// </summary>
        private void ClearActivationAndTokens()
        {
            AutoClean();
            if (GetActivationKey().Length > 0)
            {
                _conn.NonQuery("DELETE from Activation");
            }
        }

        /// <summary>
        /// Removes all licenseKeys
        /// </summary>
        /// <param name="licenseKey"></param>
        private void CheckLicenseKey(string licenseKey)
        {
            if (licenseKey.Length <= 0)
            {
                throw new NoLicenceKeyRegistredException();
            }
        }

        /// <summary>
        /// Inserts a licenseKey
        /// </summary>
        /// <param name="licenseKey">string licenseKey</param>
        public void SetLicenseKey(string licenseKey)
        {
            AutoClean();
            SQLiteCommand com = new SQLiteCommand("Insert into LicenseKey  (LicenseKey) values (@LicenseKey);");
            com.Parameters.AddWithValue("@LicenseKey", licenseKey);
            _conn.NonQuery(com);
        }

        /// <summary>
        /// Gets the latest licenseKey
        /// </summary>
        /// <returns></returns>
        private string GetLicenseKey()
        {
            AutoClean();
            DataTable dt = null;
            _conn.Query("Select LicenseKey from LicenseKey order by Id desc Limit 1", out dt);
            return dt.Rows.Count == 1 ? dt.Rows[0].Field<string>("LicenseKey") : "";
        }

        /// <summary>
        /// Inserts an ActivationKey
        /// </summary>
        /// <param name="activationKey">string activationKey</param>
        /// <param name="doe">DateOfExpiry</param>
        private void InsertActivationKey(string activationKey, DateTime doe)
        {
            AutoClean();
            SQLiteCommand com =
                new SQLiteCommand("Insert into ActivationKey  (ActivationKey, DOE) values (@ActivationKey, @DOE);");
            com.Parameters.AddWithValue("@LicenseKey", activationKey);
            com.Parameters.AddWithValue("@DOE", doe);
            _conn.NonQuery(com);
        }

        /// <summary>
        /// Checks if the activationOffer is valid
        /// </summary>
        /// <param name="mo">ManastoneActivationOffer</param>
        /// <returns>bool</returns>
        private bool CheckActivationOffer(ManastoneActivationOffer mo)
        {
            var ak = ManastoneActivationWrap.DecryptAndDeserialize(mo.ActivationKey, ManastoneTools.GetHardwareId(),
                _initializationVector);
            return ak.DateOfExpiry > DateTime.Now && ak.DateOfExpiry.CompareTo(mo.ValidUntil) > 0 &&
                   ak.programUUID == _programUUID;
        }

        /// <summary>
        /// Returns latest activation key
        /// </summary>
        /// <returns></returns>
        private string GetActivationKey()
        {
            DataTable dt = null;
            _conn.Query("SELECT ActivationKey FROM Activation ORDER BY Id desc LIMIT 1", out dt);
            return dt.Rows.Count == 1 ? dt.Rows[0].Field<string>("ActivationKey") : "";
        }

        /// <summary>
        /// CheckAndReturnActivation
        /// </summary>
        /// <returns>ManastoneActivationWrap</returns>
        private ManastoneActivationWrap CheckAndReturnActivation()
        {
            var ak = GetActivationKey();
            var maw = ManastoneActivationWrap.DecryptAndDeserialize(ak, ManastoneTools.GetHardwareId(),
                _initializationVector);
            if (maw.DateOfExpiry >= DateTime.Now && maw.programUUID != _programUUID)
            {
                ClearActivationAndTokens();
                throw new ActivationExpiredException(ak);
            }

            return maw;
        }

        /// <summary>
        /// TESTING ATM
        /// </summary>
        public void GetToken() //private after testing
        {
        }

        /// <summary>
        /// Creates an ManastoneController
        /// </summary>
        /// <param name="initializationVector">initializationVector</param>
        /// <param name="programUUID">string programUUID</param>
        // ReSharper disable once InconsistentNaming
        private ManastoneController(string initializationVector, string programUUID)
        {
            _fileKey = Sha256.GenerateSha256(ManastoneTools.GetHardwareId() + initializationVector);
            _initializationVector = initializationVector;
            _programUUID = programUUID;
            LoadDatabaseConnection();
            CreateTables();
            AutoClean();
        }

        /// <summary>
        /// TESTING ATM
        /// </summary>
        internal class ManastoneServerConnection
        {
            private string _serverAddress;

            public ManastoneServerConnection(string serverAddress)
            {
                _serverAddress = serverAddress;
            }

            public object RequestActivation()
            {
                return null;
            }

            public object RequestReActivation()
            {
                return null;
            }

            public object RequestDeactivation()
            {
                return null;
            }

            public object RequestToken()
            {
                return null;
            }

            public object RequestLicenseKeyInformation(string licenseKey)
            {
                // var licenseInformation = SocketIoClient.RetrieveSingleValue<ManastoneLicenseInformationOffer>(_serverAddress, "RequestLicenseInformation", "OfferLicenseInformation", licenseKey);
                return null;
            }
        }
    }
}
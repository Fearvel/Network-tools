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
    public class ManastoneController
    {
        private enum ActivationType {Activation, Reactivation}
        private string _initializationVector;
        private string _programUUID;
        private readonly string _fileKey;

        private string FileName => "Manastone.db";
        private SqliteConnector _conn;
        private DateTime? _activationExpiry = null;
        public DateTime? ActivationExpiry => _activationExpiry;


        public void RemoveEncryption()
        {
            _conn.SetPassword("");
        }
        private static ManastoneController _instance;

        public static ManastoneController GetInstance()
        {
            if (_instance == null)
                throw new InstanceNotSetException();
            return _instance;
        }

        // ReSharper disable once InconsistentNaming
        public static void SetInstance(string initializationVector, string programUUID)
        {
            _instance = new ManastoneController(initializationVector, programUUID);
        }

        private void LoadDatabaseConnection()
        {
            _conn = new SqliteConnector(FileName, _fileKey);
        }

        private void CreateTables()
        {
            CreateDirectoryTable();
            CreateLicenseKeyTable();
            CreateTokenTable();
            CreateActivationTable();
        }

        private void CreateDirectoryTable() =>
            _conn.NonQuery(
                "CREATE TABLE IF NOT EXISTS `Directory` (" +
                "`DKey` INTEGER," +
                "`DVal` INTEGER" +
                ");"
            );

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

        private void AutoClean() =>
            _conn.NonQuery(
                "delete from LicenseKey where DOE < date() and DOE not null; " +
                "delete from Token where DOE < date(); " +
                "delete from Activation where DOE < date(); "
            );

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



        private void Activate()
        {
            Activator(ActivationType.Activation);
        }

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
            var ow = SocketIoClient.RetrieveSingleValue<OfferWrapper<ManastoneActivationOffer>>(
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

        private void ClearActivationAndTokens()
        {
            AutoClean();
            if (GetActivationKey().Length > 0)
            {
                _conn.NonQuery("DELETE from Activation");
            }
        }

        private void CheckLicenseKey(string licenseKey)
        {
            if (licenseKey.Length <= 0)
            {
                throw new NoLicenceKeyRegistredException();
            }
        }

        public void SetLicenseKey(string licenseKey)
        {
            AutoClean();
            SQLiteCommand com = new SQLiteCommand("Insert into LicenseKey  (LicenseKey) values (@LicenseKey);");
            com.Parameters.AddWithValue("@LicenseKey", licenseKey);
            _conn.NonQuery(com);
        }

        private string GetLicenseKey()
        {
            AutoClean();
            DataTable dt = null;
            _conn.Query("Select LicenseKey from LicenseKey order by Id desc Limit 1", out dt);
            return dt.Rows.Count == 1 ? dt.Rows[0].Field<string>("LicenseKey") : "";
        }

        private void InsertActivationKey(string activationKey, DateTime doe)
        {
            AutoClean();
            SQLiteCommand com =
                new SQLiteCommand("Insert into ActivationKey  (ActivationKey, DOE) values (@ActivationKey, @DOE);");
            com.Parameters.AddWithValue("@LicenseKey", activationKey);
            com.Parameters.AddWithValue("@DOE", doe);
            _conn.NonQuery(com);
        }

        private bool CheckActivationOffer(ManastoneActivationOffer mo)
        {
            var ak = ManastoneActivationWrap.DecryptAndDeserialize(mo.ActivationKey, ManastoneTools.GetHardwareId(),
                _initializationVector);
            return ak.DateOfExpiry > DateTime.Now && ak.DateOfExpiry.CompareTo(mo.ValidUntil) > 0 &&
                   ak.programUUID == _programUUID;
        }

        private string GetActivationKey()
        {
            DataTable dt = null;
            _conn.Query("SELECT ActivationKey FROM Activation ORDER BY Id desc LIMIT 1", out dt);
            return dt.Rows.Count == 1 ? dt.Rows[0].Field<string>("ActivationKey") : "";
        }

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

        public void GetToken() //private after testing
        {
        }

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
using System.Data.SQLite;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.SocketIo;
using de.fearvel.net.DataTypes.Manastone;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.Manastone
{
    public class ManastoneController
    {
        private string FileName => "Manastone.db";
        private string FileKey;
        private SqliteConnector _conn;
        private bool _activated = false;
        public bool Activated => _activated;

        private static ManastoneController _instance;
        public static ManastoneController GetInstance()
        {
            if (_instance == null)
                throw new InstanceNotSetException();
            return _instance;
        }
        public static void SetInstance(string salt)
        {
            _instance = new ManastoneController(salt);
        }

        private void LoadDatabaseConnection()
        {
            _conn = new SqliteConnector(FileName, FileKey);
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
            if (true) //insert check
            {
                _activated = true;
            }

            return Activated;
        }

        private void Reactivate()
        {
            AutoClean();
            if (true) //insert check
            {
                _activated = true;
            }
        }

        private void Activate()
        {
            AutoClean();
            if (true) //insert check
            {
                _activated = true;
            }
        }

        
        public void SetLicenseKey(string licenseKey)
        {
            AutoClean();
            SQLiteCommand com = new SQLiteCommand("Insert into LicenseKey  (LicenseKey) values (@LicenseKey);");
            com.Parameters.AddWithValue("LicenseKey", licenseKey);
        }

        public void GetToken() //private after testing
        {

        }

        private ManastoneController(string salt)
        {
            FileKey = ManastoneTools.GetHardwareId() + salt;
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

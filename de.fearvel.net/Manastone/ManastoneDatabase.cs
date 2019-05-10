using System;
using System.Data;
using System.Data.SQLite;
using de.fearvel.net.Security;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.Manastone
{
    /// <summary>
    /// Manastone Database management class
    /// used for the local storage of the License information
    /// </summary>
    internal sealed class ManastoneDatabase
    {
        private readonly SqliteConnector _con;

        /// <summary>
        /// the SerialNumber Key can by accessed by this property if an SerialNumber key exists 
        /// </summary>
        internal string SerialNumber { get; private set; }

        /// <summary>
        /// internal FnLog 
        /// </summary>
        internal FnLog.FnLog Log {get; private set;}

        /// <summary>
        /// the ActivationKey can by accessed by this property if an ActivationKey exists 
        /// </summary>
        internal string ActivationKey { get; private set; }

        /// <summary>
        /// Token Property
        /// </summary>
        internal string Token { get; private set; }

        /// <summary>
        /// the CustomerReference Key can by accessed by this property if an CustomerReference exists 
        /// </summary>
        internal string CustomerReference { get; private set; }

        /// <summary>
        /// public constructor
        /// Creates or opens the *.db
        /// May create missing tables
        /// Uses the Cpu1 id for encryption
        /// </summary>
        internal ManastoneDatabase()
        {
            // _con = new SqliteConnector("Manastone.db"); //DEBUG
            _con = new SqliteConnector("Manastone.db", Ident.GetCPUId());
            Log = new FnLog.FnLog(new FnLog.FnLog.FnLogInitPackage("https://app.fearvel.de:9020/",
                    "MANASTONE", Version.Parse(ManastoneClient.ClientVersion), FnLog.FnLog.TelemetryType.LogLocalSendAll, "", ""),
                _con);
            CreateTables();
            LoadLicenseInformation();
           
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "Manastone DB INIT Complete","");
        }

        /// <summary>
        /// Reads the License Information from the Database and fills the properties with it
        /// </summary>
        private void LoadLicenseInformation()
        {
            
            if (LicenseInstalled())
            {
                Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "LoadLicenseInformation", "License Found");
                _con.Query("SELECT * FROM `License`;", out DataTable dt);
                var row = dt.Rows[0];
                SerialNumber = row.Field<string>("SerialNumber");
                ActivationKey = row.Field<string>("ActivationKey");
                Token = row.Field<string>("Token");
                CustomerReference = row.Field<string>("CustomerReference");
            }
            else
            {
                Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "LoadLicenseInformation", "License NOT Found");
                SerialNumber = "";
                ActivationKey = "";
                Token = "";
                CustomerReference = "";
            }
        }

        /// <summary>
        /// bool which reflects if a license is installed
        /// </summary>
        /// <returns>a bool true if license existent</returns>
        internal bool LicenseInstalled()
        {
            _con.Query("SELECT EXISTS (SELECT * FROM `License`) as LicenseInstalled;", out DataTable dt);
            var licInst = dt.Rows[0].Field<long>("LicenseInstalled") == 1;
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "LoadLicenseInformation", licInst.ToString());
            return licInst;
        }

        /// <summary>
        /// Removes all Licenses from the License Table
        /// </summary>
        private void DeleteFromLicense()
        {
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "DeleteFromLicense", "");
            _con.NonQuery("DELETE FROM `License`;");
        }

        /// <summary>
        /// Inserts a SerialNumber into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="serialNumber"></param>
        internal void InsertSerialNumber(string serialNumber)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertSerialNumber", "Start");
            if (LicenseInstalled())
                DeleteFromLicense();
            var command = new SQLiteCommand("INSERT INTO `License` (`SerialNumber`) VALUES (@SerialNumber);");
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertSerialNumber", "Complete");
            }

        /// <summary>
        /// Inserts a ActivationKey into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="serialNumber">the serialNumber for identification of the row to be used</param>
        /// <param name="activationKey">the activationKey which will be updated</param>
        internal void InsertActivationKey(string serialNumber, string activationKey)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertActivationKey", "Start");
            var command =
                new SQLiteCommand(
                    "UPDATE `License` set `ActivationKey` = @ActivationKey where `SerialNumber` = @SerialNumber;");
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertActivationKey", "Complete");
        }

        /// <summary>
        /// Inserts a Token into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="activationKey">the activationKey for identification of the row to be used</param>
        /// <param name="token">the token which will be updated</param>
        internal void InsertToken(string activationKey, string token)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertToken", "Start");
            var command =
                new SQLiteCommand("UPDATE `License` set `Token` = @Token where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertToken", "Complete");
        }

        /// <summary>
        /// Inserts a CustomerReference into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="activationKey">the activationKey for identification of the row to be used</param>
        /// <param name="customerReference">the customerReference which will be updated</param>
        internal void InsertCustomerReference(string activationKey, string customerReference)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertCustomerReference", "Start");
            var command = new SQLiteCommand("UPDATE `License` set `CustomerReference` = @CustomerReference" +
                                            " where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@CustomerReference", customerReference);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "InsertCustomerReference", "Complete");
        }


        /// <summary>
        /// function to create the Tables
        /// creates the basic tables if they are not existent
        /// </summary>
        private void CreateTables()
        {
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "CreateTables", "Start");
            CreateDirectoryTable();
            CreateLicenseTable();
            Log.AddToLogList(FnLog.FnLog.LogType.DrmDatabaseLog, "CreateTables", "Complete");

        }

        /// <summary>
        /// Creates the Directory Table if not exists
        /// which is there for later use
        /// </summary>
        private void CreateDirectoryTable()
        {
            _con.NonQuery("CREATE TABLE IF NOT EXISTS `Directory` (" +
                          " `DKey` varchar(200)," +
                          " `DVal` Text," +
                          " CONSTRAINT uq_Version_Identifier UNIQUE (`DKey`));");
        }

        /// <summary>
        /// Creates the License Table if not exists
        /// </summary>
        private void CreateLicenseTable()
        {
            _con.NonQuery("CREATE TABLE IF NOT EXISTS `License` (" +
                          "	`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,             " +
                          "	`SerialNumber`	varchar(36) NOT NULL DEFAULT '',             " +
                          "	`ActivationKey`	varchar(36) NOT NULL DEFAULT '',             " +
                          "	`Token`	varchar(36) NOT NULL DEFAULT '',                     " +
                          "	`CustomerReference`	varchar(100) NOT NULL DEFAULT ''         " +
                          ");");
        }
    }
}
using System;
using System.Data;
using System.Data.SQLite;
using de.fearvel.net.Security;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.Manastone
{
    /// <summary>
    /// Manastone Database management class
    /// </summary>
    internal sealed class ManastoneDatabase
    {
        private readonly SqliteConnector _con;

        /// <summary>
        /// SerialNumber Property
        /// </summary>
        internal string SerialNumber { get; private set; }


        internal FnLog.FnLog Log {get; private set;}

        /// <summary>
        /// ActivationKey Property
        /// </summary>
        internal string ActivationKey { get; private set; }

        /// <summary>
        /// Token Property
        /// </summary>
        internal string Token { get; private set; }

        /// <summary>
        /// CustomerReference Property
        /// </summary>
        internal string CustomerReference { get; private set; }

        /// <summary>
        /// public constructor
        /// </summary>
        internal ManastoneDatabase()
        {
            // _con = new SqliteConnector("Manastone.db"); //DEBUG
            _con = new SqliteConnector("Manastone.db", Ident.GetCPUId());
            Log = new FnLog.FnLog(new FnLog.FnLog.FnLogInitPackage("https://log.fearvel.de",
                    "MANASTONE", Version.Parse(ManastoneClient.ClientVersion), FnLog.FnLog.TelemetryType.LogLocalSendAll, "", ""),
                _con);
            CreateTables();
            LoadLicenseInformation();
           
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo,"Manastone DB INIT Complete","");
        }

        /// <summary>
        /// Reads the License Information from the Database
        /// </summary>
        private void LoadLicenseInformation()
        {
            
            if (LicenseInstalled())
            {
                Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "LoadLicenseInformation", "License Found");
                _con.Query("SELECT * FROM `License`;", out DataTable dt);
                var row = dt.Rows[0];
                SerialNumber = row.Field<string>("SerialNumber");
                ActivationKey = row.Field<string>("ActivationKey");
                Token = row.Field<string>("Token");
                CustomerReference = row.Field<string>("CustomerReference");
            }
            else
            {
                Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "LoadLicenseInformation", "License NOT Found");
                SerialNumber = "";
                ActivationKey = "";
                Token = "";
                CustomerReference = "";
            }
        }

        /// <summary>
        /// bool which reflects if a license is installed
        /// </summary>
        /// <returns></returns>
        internal bool LicenseInstalled()
        {
            _con.Query("SELECT EXISTS (SELECT * FROM `License`) as LicenseInstalled;", out DataTable dt);
            var licInst = dt.Rows[0].Field<long>("LicenseInstalled") == 1;
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "LoadLicenseInformation", licInst.ToString());
            return licInst;
        }

        /// <summary>
        /// Removes all Licenses from the License Table
        /// </summary>
        private void DeleteFromLicense()
        {
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "DeleteFromLicense", "");
            _con.NonQuery("DELETE FROM `License`;");
        }

        /// <summary>
        /// Inserts a SerialNumber into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="serialNumber"></param>
        internal void InsertSerialNumber(string serialNumber)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertSerialNumber", "Start");
            if (LicenseInstalled())
                DeleteFromLicense();
            var command = new SQLiteCommand("INSERT INTO `License` (`SerialNumber`) VALUES (@SerialNumber);");
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertSerialNumber", "Complete");
            }

        /// <summary>
        /// Inserts a ActivationKey into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="activationKey"></param>
        internal void InsertActivationKey(string serialNumber, string activationKey)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertActivationKey", "Start");
            var command =
                new SQLiteCommand(
                    "UPDATE `License` set `ActivationKey` = @ActivationKey where `SerialNumber` = @SerialNumber;");
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertActivationKey", "Complete");
        }

        /// <summary>
        /// Inserts a Token into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="token"></param>
        internal void InsertToken(string activationKey, string token)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertToken", "Start");
            var command =
                new SQLiteCommand("UPDATE `License` set `Token` = @Token where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertToken", "Complete");
        }

        /// <summary>
        /// Inserts a CustomerReference into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="customerReference"></param>
        internal void InsertCustomerReference(string activationKey, string customerReference)
        {
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertCustomerReference", "Start");
            var command = new SQLiteCommand("UPDATE `License` set `CustomerReference` = @CustomerReference" +
                                            " where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@CustomerReference", customerReference);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "InsertCustomerReference", "Complete");
        }


        /// <summary>
        /// function to create the Tables
        /// </summary>
        private void CreateTables()
        {
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "CreateTables", "Start");
            CreateDirectoryTable();
            CreateLicenseTable();
            Log.AddToLogList(FnLog.FnLog.LogType.RuntimeInfo, "CreateTables", "Complete");

        }

        /// <summary>
        /// Creates the Directory Table
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
        /// Creates the License Table
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
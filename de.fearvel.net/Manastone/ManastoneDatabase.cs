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
            CreateTables();
            LoadLicenseInformation();
        }

        /// <summary>
        /// Reads the License Information from the Database
        /// </summary>
        private void LoadLicenseInformation()
        {
            if (LicenseInstalled())
            {
                _con.Query("SELECT * FROM `License`;", out DataTable dt);
                var row = dt.Rows[0];
                SerialNumber = row.Field<string>("SerialNumber");
                ActivationKey = row.Field<string>("ActivationKey");
                Token = row.Field<string>("Token");
                CustomerReference = row.Field<string>("CustomerReference");
            }
            else
            {
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
            return dt.Rows[0].Field<long>("LicenseInstalled") == 1 ? true : false;
        }

        /// <summary>
        /// Removes all Licenses from the License Table
        /// </summary>
        private void DeleteFromLicense()
        {
            _con.NonQuery("DELETE FROM `License`;");
        }

        /// <summary>
        /// Inserts a SerialNumber into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="serialNumber"></param>
        internal void InsertSerialNumber(string serialNumber)
        {
            if (LicenseInstalled())
                DeleteFromLicense();
            var command = new SQLiteCommand("INSERT INTO `License` (`SerialNumber`) VALUES (@SerialNumber);");
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }

        /// <summary>
        /// Inserts a ActivationKey into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="activationKey"></param>
        internal void InsertActivationKey(string serialNumber, string activationKey)
        {
            var command =
                new SQLiteCommand(
                    "UPDATE `License` set `ActivationKey` = @ActivationKey where `SerialNumber` = @SerialNumber;");
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }

        /// <summary>
        /// Inserts a Token into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="token"></param>
        internal void InsertToken(string activationKey, string token)
        {
            var command =
                new SQLiteCommand("UPDATE `License` set `Token` = @Token where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }

        /// <summary>
        /// Inserts a CustomerReference into the Database
        /// also triggers LoadLicenseInformation();
        /// </summary>
        /// <param name="activationKey"></param>
        /// <param name="customerReference"></param>
        internal void InsertCustomerReference(string activationKey, string customerReference)
        {
            var command = new SQLiteCommand("UPDATE `License` set `CustomerReference` = @CustomerReference" +
                                            " where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@CustomerReference", customerReference);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }


        /// <summary>
        /// function to create the Tables
        /// </summary>
        private void CreateTables()
        {
            CreateDirectoryTable();
            CreateLicenseTable();
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
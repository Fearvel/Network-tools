using System.Data;
using System.Data.SQLite;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.Manastone
{
    public sealed class ManastoneDatabase
    {
        private readonly SqliteConnector _con;

        public string SerialNumber { get; private set; }
        public string ActivationKey { get; private set; }
        public string Token { get; private set; }


        public ManastoneDatabase()
        {
            _con = new SqliteConnector("Manastone.db"); //DEBUG
            //    _con = new SqliteConnector("Manastone.db", Ident.GetCPUId());
            CreateTables();
        }


        public void LoadLicenseInformation()
        {
            if (LicenseInstalled())
            {
                _con.Query("SELECT * FROM `License`;", out DataTable dt);
                var row = dt.Rows[0];
                SerialNumber = row.Field<string>("SerialNumber");
                ActivationKey = row.Field<string>("ActivationKey");
                Token = row.Field<string>("Token");
            }
            SerialNumber = "";
            ActivationKey = "";
            Token = "";
        }


        public bool LicenseInstalled()
        {
            _con.Query("SELECT EXISTS (SELECT * FROM `License`) as LicenseInstalled;", out DataTable dt);
            return dt.Rows[0].Field<long>("LicenseInstalled") == 1 ? true : false;
        }

        private void DeleteFromLicense()
        {
            _con.NonQuery("DELETE FROM `License`;");
        }



        public void InsertSerialNumber(string serialNumber)
        {
            if (LicenseInstalled())
                DeleteFromLicense();
            var command = new SQLiteCommand("INSERT INTO `License` (`SerialNumber`) VALUES (@SerialNumber);");
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }

        public void InsertActivationKey(string serialNumber, string activationKey)
        {
            var command = new SQLiteCommand("UPDATE `License` set `ActivationKey` = @ActivationKey where `SerialNumber` = @SerialNumber;");
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }

        public void InsertToken(string activationKey, string token)
        {
            var command = new SQLiteCommand("UPDATE `License` set `Token` = @Token where `ActivationKey` = @ActivationKey;");
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ActivationKey", activationKey);
            command.Prepare();
            _con.NonQuery(command);
            LoadLicenseInformation();
        }




        private void CreateTables()
        {
            CreateDirectoryTable();
            CreateLicenseTable();
        }

        public void CreateDirectoryTable()
        {
            _con.NonQuery("CREATE TABLE IF NOT EXISTS `Directory` (" +
                          " `DKey` varchar(200)," +
                          " `DVal` Text," +
                          " CONSTRAINT uq_Version_Identifier UNIQUE (`DKey`));");
        }

        public void CreateLicenseTable()
        {
            _con.NonQuery("CREATE TABLE IF NOT EXISTS `License` (" +
                          "	`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                          "	`SerialNumber` varchar(36) NOT NULL DEFAULT ''," +
                          "	`ActivationKey`	varchar(36) NOT NULL DEFAULT ''," +
                          "	`Token`	varchar(36) NOT NULL DEFAULT ''" +
                          ");");
        }
    }
}
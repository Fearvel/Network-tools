using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.FnLog
{
    /// <summary>
    /// FnLog Local Logging Controller
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class LocalLogger
    {
        /// <summary>
        /// SqliteConnector
        /// </summary>
        internal SqliteConnector Connection { private set; get; }

        /// <summary>
        /// UUID of the Logger (A Random UUID will be generated if none exists)
        /// </summary>
        internal Guid UUID { private set; get; }

        /// <summary>
        /// Getter for the UUID
        /// </summary>
        /// <returns>UUID</returns>
        internal Guid GetUUID()
        {
            Connection.Query("Select DVal from Directory where DKey = 'LoggerUUID';", out DataTable dt);
            if (dt.Rows.Count == 0 || !Guid.TryParse(dt.Rows[0].Field<string>("DVal"), out var res))
                return Guid.Empty;
            return res;
        }

        /// <summary>
        /// Constructor
        /// calls the LocalLogger(string fileName, string encKey) constructor with encKey = ""
        /// </summary>
        /// <param name="fileName"></param>
        public LocalLogger(string fileName) : this(fileName, "")
        {
        }

        /// <summary>
        /// Constructor
        /// Creates a Database Connection
        /// Creates Tables if not Exists
        /// fills the UUID property
        /// </summary>
        /// <param name="fileName">FileLocation</param>
        /// <param name="encKey">Password</param>
        public LocalLogger(string fileName, string encKey)
        {
            Connection = new SqliteConnector(fileName, encKey);
            CreateTables();
            UUID = GetUUID();
        }

        /// <summary>
        /// Constructor
        /// uses an Existent database connection
        /// Creates Tables if not Exists
        /// fills the UUID property
        /// </summary>
        /// <param name="con">DB connection</param>
        public LocalLogger(SqliteConnector con)
        {
            Connection = con;
            CreateTables();
            UUID = GetUUID();
        }

        /// <summary>
        /// Creates Tables if not exist
        /// </summary>
        private void CreateTables()
        {
            CreateInformationTable();
            CreateErrorTable();
        }

        /// <summary>
        /// CreateInformationTable
        /// Fill with defined values
        /// </summary>
        private void CreateInformationTable()
        {
            Connection.NonQuery("CREATE TABLE IF NOT EXISTS Directory" +
                                " (DKey varchar(200),DVal Text," +
                                " CONSTRAINT uq_Version_Identifier UNIQUE (DKey));");

            Connection.Query("Select * from Directory where DKey = 'LoggerVersion'", out DataTable dtVersion);
            if (dtVersion.Rows.Count == 0)
            {
                Connection.NonQuery("INSERT INTO Directory (DKey,DVal) VALUES ('LoggerVersion'," +
                                    "'" + FileVersionInfo
                                        .GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)
                                        .ProductVersion + "');");
            }

            Connection.Query("Select * from Directory where DKey = 'LoggerUUID'", out DataTable dtUuid);
            if (dtUuid.Rows.Count == 0)
            {
                Connection.NonQuery("INSERT INTO Directory (DKey,DVal) VALUES ('LoggerUUID','" +
                                    Guid.NewGuid().ToString() +
                                    "');");
            }
        }

        /// <summary>
        /// CreateErrorTable
        /// Create 2 Triggers
        /// </summary>
        private void CreateErrorTable()
        {
            Connection.NonQuery("CREATE TABLE IF NOT EXISTS Log" +
                                " (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                " LogType int NOT NULL," +
                                " Title varchar(200) NOT NULL DEFAULT ''," +
                                " Description text NOT NULL DEFAULT ''," +
                                " DateTimeOfIncident Timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP);");
            Connection.NonQuery("CREATE TRIGGER IF NOT EXISTS removeOldRuntimeInfoOnInsert After INSERT ON Log" +
                                " BEGIN Delete from Log where DateTimeOfIncident <= date('now','-30 day'); END");
        }

        /// <summary>
        /// Adds a log to the Databases
        /// </summary>
        /// <param name="logType">logType</param>
        /// <param name="title">title</param>
        /// <param name="description">description</param>
        public void AddLog(int logType, string title, string description)
        {
            using (var command = new SQLiteCommand(
                "Insert Into Log(LogType,Title,Description) values (@LogType,@Title,@Description)")
            )
            {
                command.Parameters.AddWithValue("@LogType", logType);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Description", description);
                command.Prepare();
                Connection.NonQuery(command);
            }
        }

        /// <summary>
        /// BoolToInt conversion
        /// </summary>
        /// <param name="b">bool</param>
        /// <returns>int</returns>
        private int BoolToInt(bool b) => b ? 1 : 0;

        /// <summary>
        /// Adds a log to the Databases
        /// </summary>
        /// <param name="logType">logType</param>
        /// <param name="title">title</param>
        /// <param name="description">description</param>
        public void AddLog(FnLog.LogType logType, string title, string description) =>
            AddLog((int)logType, title, description);


        /// <summary>
        /// Returns a DataTable containing Errors and Warnings
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetErrorsAndWarnings()
        {
            Connection.Query(
                "Select log.LogType, log.Title, log.Description, log.DateTimeOfIncident, version.DVal as Version, UUID.DVal as UUID from" +
                " Log log left join Directory version left join Directory UUID " +
                "where version.DKey = 'LoggerVersion' and UUID.DKey = 'LoggerUUID' and ( LogType = "
                + (int) FnLog.LogType.CriticalError + " or LogType = "
                + (int) FnLog.LogType.Error + " or LogType = "
                + (int) FnLog.LogType.Warning + " or LogType = "
                + (int) FnLog.LogType.Notice + ");", out DataTable dt);
        return dt;
        }

        /// <summary>
        /// Returns Logs of a specific Type as DataTable
        /// </summary>
        /// <param name="t">FnLog.LogType</param>
        /// <returns>DataTable</returns>
        public DataTable GetLog(FnLog.LogType t)
        {
            Connection.Query("Select * from Log where LogType = '" + t.ToString() + "';", out DataTable dt);
            return dt;
        }

        /// <summary>
        /// Returns all logs as DataTable
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetLog()
        {
            Connection.Query("Select * from Log order by Id desc;", out DataTable dt);
            return dt;
        }
    }
}
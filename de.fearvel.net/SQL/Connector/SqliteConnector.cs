using System.Data;
using System.Data.SQLite;

namespace de.fearvel.net.SQL.Connector
{
    /// <summary>
    /// Connector for SQLite
    /// </summary>
    public class SqliteConnector : SqlConnector
    {
        /// <summary>
        /// Constructor
        /// Connects to a database with the ConnectToDatabase function
        /// </summary>
        /// <param name="fileName"></param>
        public SqliteConnector(string fileName) =>
            ConnectToDatabase(fileName);

        /// <summary>
        /// Constructor
        /// Connects to a database with the ConnectToDatabase function
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="enckey"></param>
        public SqliteConnector(string fileName, string enckey) =>
            ConnectToDatabase(fileName, enckey);


        /// <summary>
        /// Creates A Database Connection
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="enckey"></param>
        public void ConnectToDatabase(string fileName, string enckey = "")
        {
            ConStr = new SQLiteConnectionStringBuilder
            {
                DataSource = fileName,
                Version = 3,
                Password = enckey
            };
            Connect = new SQLiteConnection(ConStr.ConnectionString);
        }

        /// <summary>
        /// Sets a Password for the SQLite Database
        /// </summary>
        /// <param name="pass"></param>
        public void SetPassword(string pass)
        {
            ((SQLiteConnection)Connect).Open();
            ((SQLiteConnection) Connect).ChangePassword(pass);
            ((SQLiteConnection)Connect).Close();
        }

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="dt"></param>
        public override void Query(string sqlCmd, out DataTable dt) =>
            base.Query(new SQLiteCommand(sqlCmd), out dt);

        /// <summary>
        /// SQL Query
        /// out DataSet
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="ds"></param>
        public override void Query(string sqlCmd, out DataSet ds) =>
            base.Query(new SQLiteCommand(sqlCmd), out ds);

        public SQLiteDataAdapter Query(string sqlCmd)
        {
            return Query(new SQLiteCommand(sqlCmd));
        }

        public SQLiteDataAdapter Query(SQLiteCommand command)
        {
            command.Connection = (SQLiteConnection)Connect;
            return new SQLiteDataAdapter(command);
        }

        public void Update(SQLiteDataAdapter da, DataSet ds)
        {
            var changes = ds.GetChanges();
            if (changes != null)
            {
                da.UpdateCommand = new SQLiteCommandBuilder(da).GetUpdateCommand();
                da.Update(changes);
                ds.AcceptChanges();
            }
        }


        /// <summary>
        /// SQL NonQuery
        /// </summary>
        /// <param name="sqlCmd"></param>
        public override void NonQuery(string sqlCmd) =>
            base.NonQuery(new SQLiteCommand(sqlCmd));

        /// <summary>
        /// Getter for a SQLiteConnection
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection GetConnection() => (SQLiteConnection)Connect;
    }
}
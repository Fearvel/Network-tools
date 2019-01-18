using System.Data;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace de.fearvel.net.SQL.Connector
{
    /// <summary>
    /// Connector for MySQL
    /// </summary>
    public class MysqlConnector : SqlConnector
    {
        /// <summary>
        /// Constructor
        /// Connects to a database with the ConnectToDatabase function
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sslm"></param>
        public MysqlConnector(string address, int port, string database,
            string username, string password, bool sslm) =>
            ConnectToDatabase(address, port, database, username, password, sslm);
        
        /// <summary>
        /// Creates A Database Connection
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sslm"></param>
        private void ConnectToDatabase(string address, int port, string database,
            string username, string password, bool sslm)
        {
            ConStr = new MySqlConnectionStringBuilder
            {
                UserID = username,
                Password = password,
                Server = address,
                Port = (uint)port,
                Database = database,
                SslMode = sslm ? MySqlSslMode.Required : MySqlSslMode.None
            };
            Connect = new MySqlConnection(ConStr.ConnectionString);
        }

        /// <summary>
        /// SQL Query
        /// out DataSet
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="ds"></param>
        public override void Query(string sqlCmd, out DataSet ds) =>
            base.Query(new MySqlCommand(sqlCmd), out ds);

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="dt"></param>
        public override void Query(string sqlCmd, out DataTable dt) =>
            base.Query(new MySqlCommand(sqlCmd), out dt);

        public MySqlDataAdapter Query(string sqlCmd)
        {
            return Query(new MySqlCommand(sqlCmd));
        }

        public MySqlDataAdapter Query(MySqlCommand command)
        {
            command.Connection = (MySqlConnection)Connect;
            return new MySqlDataAdapter(command);
        }

        public void Update(MySqlDataAdapter da, DataSet ds)
        {
            var changes = ds.GetChanges();
            if (changes != null)
            {
                da.UpdateCommand = new MySqlCommandBuilder(da).GetUpdateCommand();
                da.Update(changes);
                ds.AcceptChanges();
            }
        }

        /// <summary>
        /// SQL NonQuery
        /// </summary>
        /// <param name="sqlCmd"></param>
        public override void NonQuery(string sqlCmd) =>
            base.NonQuery(new MySqlCommand(sqlCmd));

        /// <summary>
        /// Getter for a MySqlConnection
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnection() => (MySqlConnection)Connect;
    }
}
using System.Data;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace de.fearvel.net.SQL.Connector
{
    /// <summary>
    /// Connector for MySQL
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class MysqlConnector : SqlConnector
    {
        /// <summary>
        /// Constructor
        /// Connects to a database with the ConnectToDatabase function
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="port">port</param>
        /// <param name="database">database</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="sslm">sslMode</param>
        public MysqlConnector(string address, int port, string database,
            string username, string password, bool sslm) =>
            ConnectToDatabase(address, port, database, username, password, sslm);

        /// <summary>
        /// Creates A Database Connection
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="port">port</param>
        /// <param name="database">database</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="sslm">sslMode</param>
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
        /// <param name="sqlCmd">sql command string</param>
        /// <param name="ds">out DataSet</param>
        public override void Query(string sqlCmd, out DataSet ds) =>
            base.Query(new MySqlCommand(sqlCmd), out ds);

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        /// <param name="dt">out DataTable</param>
        public override void Query(string sqlCmd, out DataTable dt) =>
            base.Query(new MySqlCommand(sqlCmd), out dt);

        /// <summary>
        /// Query that returns a MySqlDataAdapter
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        /// <returns></returns>
        public MySqlDataAdapter Query(string sqlCmd)
        {
            return Query(new MySqlCommand(sqlCmd));
        }

        /// <summary>
        /// Query that returns a MySqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        /// <returns>MySqlCommand</returns>
        public MySqlDataAdapter Query(MySqlCommand command)
        {
            command.Connection = (MySqlConnection)Connect;
            return new MySqlDataAdapter(command);
        }

        /// <summary>
        /// Updates the Database
        /// </summary>
        /// <param name="da">MySqlDataAdapter</param>
        /// <param name="ds">DataSet</param>
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
        /// <param name="sqlCmd">sql command string</param>
        public override void NonQuery(string sqlCmd) =>
            base.NonQuery(new MySqlCommand(sqlCmd));

        /// <summary>
        /// Getter for a MySqlConnection
        /// </summary>
        /// <returns>MySqlConnection</returns>
        public MySqlConnection GetConnection() => (MySqlConnection)Connect;
    }
}
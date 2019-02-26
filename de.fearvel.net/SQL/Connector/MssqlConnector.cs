using System.Data;
using System.Data.SqlClient;

namespace de.fearvel.net.SQL.Connector
{
    /// <summary>
    /// Connector for MSSQL
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class MssqlConnector : SqlConnector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MssqlConnector" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="database">The database.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public MssqlConnector(string serverName, string database,
            string username, string password) =>
            ConnectToDatabase(serverName, database, username, password);

        /// <summary>
        /// Initializes a new instance of the <see cref="MssqlConnector" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="database">The database.</param>
        public MssqlConnector(string serverName, string database) =>
            ConnectToDatabase(serverName, database);

        /// <summary>
        /// Creates A Database Connection
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="database">database</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public void ConnectToDatabase(string address, string database,
            string username, string password)
        {
            ConStr = new SqlConnectionStringBuilder
            {
                DataSource = address + "\\",
                UserID = username,
                Password = password,
                ["Database"] = database,
                TrustServerCertificate = true,
                IntegratedSecurity = false,
            };
            Connect = new SqlConnection(ConStr.ConnectionString);
        }

        /// <summary>
        /// Creates A Database Connection Windows auth
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="database">database</param>
        public void ConnectToDatabase(string address, string database)
        {
            ConStr = new SqlConnectionStringBuilder
            {
                DataSource = address + "\\",
                ["Database"] = database,
                TrustServerCertificate = true,
                IntegratedSecurity = true,
            };
            Connect = new SqlConnection(ConStr.ConnectionString);
        }

        /// <summary>
        /// SQL Query
        /// out DataSet
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        /// <param name="ds">out DataSet</param>
        public override void Query(string sqlCmd, out DataSet ds) =>
            base.Query(new SqlCommand(sqlCmd), out ds);

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        /// <param name="dt">out DataTable</param>
        public override void Query(string sqlCmd, out DataTable dt) =>
            base.Query(new SqlCommand(sqlCmd), out dt);

        /// <summary>
        /// Query that returns a SqlDataAdapter
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        /// <returns>SqlDataAdapter</returns>
        public SqlDataAdapter Query(string sqlCmd)
        {
            return Query(new SqlCommand(sqlCmd, (SqlConnection) Connect));
        }

        /// <summary>
        /// Query that returns a SqlDataAdapter
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>SqlDataAdapter</returns>
        public SqlDataAdapter Query(SqlCommand command)
        {
            command.Connection = (SqlConnection) Connect;
            return new SqlDataAdapter(command);
        }

        /// <summary>
        /// Updates the Database
        /// </summary>
        /// <param name="da">SqlDataAdapter</param>
        /// <param name="ds">DataSet</param>
        public void Update(SqlDataAdapter da, DataSet ds)
        {
            var changes = ds.GetChanges();
            if (changes != null)
            {
                da.UpdateCommand = new SqlCommandBuilder(da).GetUpdateCommand();
                da.Update(changes);
                ds.AcceptChanges();
            }
        }

        /// <summary>
        /// SQL NonQuery
        /// </summary>
        /// <param name="sqlCmd">sql command string</param>
        public override void NonQuery(string sqlCmd) =>
            base.NonQuery(new SqlCommand(sqlCmd));

        /// <summary>
        /// Getter for a SqlConnection
        /// </summary>
        /// <returns>SqlConnection</returns>
        public SqlConnection GetConnection() => (SqlConnection) Connect;
    }
}
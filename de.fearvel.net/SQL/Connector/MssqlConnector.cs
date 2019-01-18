using System.Data;
using System.Data.SqlClient;

namespace de.fearvel.net.SQL.Connector
{
    /// <summary>
    /// Connector for MSSQL
    /// </summary>
    public class MssqlConnector : SqlConnector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MssqlConnector" /> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="instance"> Instance name</param>
        /// <param name="database">The database.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="sslmode"> true if it is encrypted</param>
        public MssqlConnector(string serverName, string instance, string database,
            string username, string password, bool sslmode) =>
            ConnectToDatabase(serverName, instance, database, username, password, sslmode);

        /// <summary>
        /// Creates A Database Connection
        /// </summary>
        /// <param name="address"></param>
        /// <param name="instance"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="sslmode"></param>
        public void ConnectToDatabase(string address, string instance, string database,
            string username, string password, bool sslmode)
        {
            ConStr = new SqlConnectionStringBuilder
            {
                DataSource = address + "\\",
                UserID = username,
                Password = password,
                ["Database"] = database,
                TrustServerCertificate = true,
                IntegratedSecurity = sslmode,
            };
            Connect = new SqlConnection(ConStr.ConnectionString);
        }

        /// <summary>
        /// SQL Query
        /// out DataSet
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="ds"></param>
        public override void Query(string sqlCmd, out DataSet ds) =>
            base.Query(new SqlCommand(sqlCmd), out ds);

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <param name="dt"></param>
        public override void Query(string sqlCmd, out DataTable dt) =>
            base.Query(new SqlCommand(sqlCmd), out dt);

        public SqlDataAdapter Query(string sqlCmd)
        {
            return Query(new SqlCommand());
        }

        public SqlDataAdapter Query(SqlCommand command)
        {
            command.Connection = (SqlConnection)Connect;
            return new SqlDataAdapter(command);
        }

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
        /// <param name="sqlCmd"></param>
        public override void NonQuery(string sqlCmd) =>
            base.NonQuery(new SqlCommand(sqlCmd));

        /// <summary>
        /// Getter for a SqlConnection
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnection() => (SqlConnection)Connect;
    }
}

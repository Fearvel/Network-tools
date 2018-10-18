using System.Data;
using System.Data.SqlClient;

namespace de.fearvel.net.SQL.Connector
{
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
        public MssqlConnector(string serverName, string instance, string database, string username, string password, bool sslmode)
        {
            ConnectToDatabase(serverName, instance, database, username, password, sslmode);
        }

        public void ConnectToDatabase(string address, string instance, string database, string username, string password, bool sslmode)
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
            Connect.Open();
        }

        public override void Query(string sqlCmd, out DataSet ds)
        {
            base.Query(new SqlCommand(sqlCmd), out ds);
        }
        public override void Query(string sqlCmd, out DataTable dt)
        {
            base.Query(new SqlCommand(sqlCmd), out dt);
        }

        public override void NonQuery(string sqlCmd)
        {
            base.NonQuery(new SqlCommand(sqlCmd));
        }

        public SqlConnection GetConnection()
        {
            return (SqlConnection)Connect;
        }
    }
}

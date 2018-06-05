using System.Data;
using MySql.Data.MySqlClient;

namespace de.fearvel.net.SQL.Connector
{
    public class MysqlConnector : SqlConnector
    {
        public MysqlConnector(string address, int port, string database, string username, string password, bool sslm)
        {
            ConnectToDatabase(address, port, database, username, password, sslm);
        }

        private void ConnectToDatabase(string address, int port, string database, string username, string password, bool sslm)
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
            Connect.Open();
        }

        public override DataTable Query(string sqlCmd)
        {
            return base.Query(new MySqlCommand(sqlCmd, (MySqlConnection)Connect));
        }

        public override void NonQuery(string sqlCmd)
        {
            base.NonQuery(new MySqlCommand(sqlCmd, (MySqlConnection)Connect));
        }
    }
}

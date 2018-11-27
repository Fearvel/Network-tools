using System.Data;
using MySql.Data.MySqlClient;

namespace de.fearvel.net.SQL.Connector
{
    public class MysqlConnector : SqlConnector
    {
        public MysqlConnector(string address, int port, string database,
            string username, string password, bool sslm) =>
            ConnectToDatabase(address, port, database, username, password, sslm);


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

        public override void Query(string sqlCmd, out DataSet ds) =>
            base.Query(new MySqlCommand(sqlCmd), out ds);

        public override void Query(string sqlCmd, out DataTable dt) =>
            base.Query(new MySqlCommand(sqlCmd), out dt);


        public override void NonQuery(string sqlCmd) =>
            base.NonQuery(new MySqlCommand(sqlCmd));

        public MySqlConnection GetConnection() => (MySqlConnection)Connect;
    }
}

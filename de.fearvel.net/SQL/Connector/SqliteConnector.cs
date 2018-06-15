using System.Data;
using System.Data.SQLite;

namespace de.fearvel.net.SQL.Connector
{
    public class SqliteConnector : SqlConnector
    {
        public SqliteConnector(string fileName)
        {
            ConnectToDatabase(fileName);
        }

        public SqliteConnector(string fileName, string enckey)
        {
            ConnectToDatabase(fileName, enckey);
        }

        public void ConnectToDatabase(string fileName, string enckey = "")
        {
            ConStr = new SQLiteConnectionStringBuilder
            {
                DataSource = fileName,
                Version = 3,
                Password = enckey
            };
            Connect = new SQLiteConnection(ConStr.ConnectionString);
            Connect.Open();
        }

        public void SetPassword(string pass)
        {
            ((SQLiteConnection)Connect).ChangePassword(pass);
        }

        public override DataTable Query(string sqlCmd)
        {
            return base.Query(new SQLiteCommand(sqlCmd, (SQLiteConnection)Connect));
        }

        public override void NonQuery(string sqlCmd)
        {
            base.NonQuery(new SQLiteCommand(sqlCmd, (SQLiteConnection)Connect));
        }

        public SQLiteConnection GetConnection()
        {
            return (SQLiteConnection) Connect;
        }
    }
}

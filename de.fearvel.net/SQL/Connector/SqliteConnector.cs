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

        public override void Query(string sqlCmd, out  DataTable dt)
        {
            base.Query(new SQLiteCommand(sqlCmd),out dt);
        }
        public override void Query(string sqlCmd, out DataSet ds)
        {
            base.Query(new SQLiteCommand(sqlCmd), out ds);
        }

        public override void NonQuery(string sqlCmd)
        {
            base.NonQuery(new SQLiteCommand(sqlCmd));
        }

        public SQLiteConnection GetConnection()
        {
            return (SQLiteConnection) Connect;
        }
    }
}

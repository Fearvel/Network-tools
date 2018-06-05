using System;
using System.Data;
using System.Data.Common;

namespace de.fearvel.net.SQL.Connector
{
    public abstract class SqlConnector
    {
        protected DbConnection Connect = null;
        protected DbConnectionStringBuilder ConStr;

        public DataTable Query(DbCommand com)
        {
            var command = com;
            var dt = new DataTable();
            dt.Load(command.ExecuteReader());
            return dt;
        }
        public void NonQuery(DbCommand com)
        {
            var command = com;
            command.ExecuteNonQuery();
        }
        public void Close()
        {
            if (IsOpen)
            {
                Connect.Close();
                Connect = null;
                GC.Collect();
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public bool IsOpen => Connect != null;
        public abstract DataTable Query(string sqlCmd);
        public abstract void NonQuery(string sqlCmd);
    }
}

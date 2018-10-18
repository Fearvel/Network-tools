using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace de.fearvel.net.SQL.Connector
{
    public abstract class SqlConnector
    {
        protected DbConnection Connect = null;
        protected DbConnectionStringBuilder ConStr;

        public void Query(DbCommand com, out DataTable dt)
        {
            com.Connection = Connect;
            dt = new DataTable();
            dt.Load(com.ExecuteReader());
        }

        public void Query(DbCommand com, out DataSet ds)
        {
            com.Connection = Connect;
            var da = new SqlDataAdapter();
            com.Connection = Connect;
            da.SelectCommand = (SqlCommand)com;
            ds = new DataSet();
            da.Fill(ds);
        }

        public void NonQuery(DbCommand com)
        {
            com.Connection = Connect;
            com.ExecuteNonQuery();
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
        public abstract void Query(string sqlCmd, out DataTable dt);
        public abstract void Query(string sqlCmd, out DataSet ds);
        public abstract void NonQuery(string sqlCmd);

    }
}

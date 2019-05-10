using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace de.fearvel.net.SQL.Connector
{
    /// <summary>
    /// Abstract Class to create SqlConnections
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public abstract class SqlConnector
    {
        /// <summary>
        /// DB Connection       
        /// </summary>
        protected DbConnection Connect = null;

        /// <summary>
        /// DbConnectionStringBuilder
        /// </summary>
        protected DbConnectionStringBuilder ConStr; 
        
        /// <summary>
        /// Prop for checking if the connection is open
        /// </summary>
        public bool IsOpen => Connect != null;

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="sqlCmd">Sql command string</param>
        /// <param name="dt">out DataTable</param>
        public abstract void Query(string sqlCmd, out DataTable dt);

        /// <summary>
        /// SQL Query
        /// out DataSet
        /// </summary>
        /// <param name="sqlCmd">Sql command string</param>
        /// <param name="ds">out DataSet</param>
        public abstract void Query(string sqlCmd, out DataSet ds);

        /// <summary>
        /// SQL NonQuery
        /// </summary>
        /// <param name="sqlCmd">Sql command string</param>
        public abstract void NonQuery(string sqlCmd);

        /// <summary>
        /// SQL Query
        /// out DataTable
        /// </summary>
        /// <param name="com">DbCommand</param>
        /// <param name="dt">out DataTable</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Query(DbCommand com, out DataTable dt)
        {
            if (Connect.State == ConnectionState.Closed)
                Connect.Open();

            com.Connection = Connect;
            dt = new DataTable();
            dt.Load(com.ExecuteReader());
            Connect.Close();
        }

        /// <summary>
        /// SQL Query
        /// out DataSet
        /// </summary>
        /// <param name="com">DbCommand</param>
        /// <param name="ds">out DataSet</param>
        public void Query(DbCommand com, out DataSet ds)
        {
            if (Connect.State == ConnectionState.Closed)
                Connect.Open();
            
            com.Connection = Connect;
            var da = new SqlDataAdapter();
            com.Connection = Connect;
            da.SelectCommand = (SqlCommand)com;
            ds = new DataSet();
            da.Fill(ds);
            Connect.Close();
        }

        /// <summary>
        /// SQL NonQuery
        /// </summary>
        /// <param name="com">DbCommand</param>
        public void NonQuery(DbCommand com)
        {
            if (Connect.State == ConnectionState.Closed)
                Connect.Open();
            
            com.Connection = Connect;
            com.ExecuteNonQuery();
            Connect.Close();
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
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
    }
}
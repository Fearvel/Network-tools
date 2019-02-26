using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.Gui.wpf
{
    /// <summary>
    /// FnLogViewer
    /// A simple way to display the recorded Logs to the user.
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class FnLogViewer
    {
        public DockPanel FnLogTable { get; private set; }

        /// <summary>
        /// Constructor
        /// Creates A DockPanel whit a DataGrid inside.
        /// The DataGrid will be filled with the Recorded logs.
        /// The Enum like LogType will be decoded to a easy readable string.
        /// </summary>
        /// <param name="con"></param>
        internal FnLogViewer(SqliteConnector con)
        {
            FnLogTable = new DockPanel()
            {
                Background = Brushes.White
            };
            var fnLogDataGrid = new DataGrid()
            {
                IsReadOnly = true,
                CanUserAddRows = false
            };
            FnLogTable.Children.Add(fnLogDataGrid);
            con.Query("Select id, LogType, Title, Description, DateTimeOfIncident, Reported from Log order by id desc",
                out DataTable dtRes);
            var resultTable = CreateResultTable();
            foreach (DataRow dr in dtRes.Rows)
            {
                resultTable.Rows.Add(dr[0], FnLog.FnLog.ParseLogType((int) dr[1]), dr[2], dr[3], dr[4], dr[5]);
            }
            fnLogDataGrid.ItemsSource = resultTable.DefaultView;
        }

        /// <summary>
        /// CreateResultTable()
        /// Creates a Template of the DataTable which will be used to display the Result
        /// </summary>
        /// <returns>DataTable</returns>
        private DataTable CreateResultTable() => new DataTable()
        {
            Columns =
            {
                new DataColumn("ID", typeof(int)),
                new DataColumn("LogType", typeof(string)),
                new DataColumn("Title", typeof(string)),
                new DataColumn("Description", typeof(string)),
                new DataColumn("Date", typeof(DateTime)),
                new DataColumn("ErrorReported", typeof(string))

            }
        };
    }
}
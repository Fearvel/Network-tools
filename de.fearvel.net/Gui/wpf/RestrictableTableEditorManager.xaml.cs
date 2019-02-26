using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using de.fearvel.net.DataTypes;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.Gui.wpf
{
    /// <summary>
    /// Interaktionslogik für RestrictableTableEditorManager.xaml
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public partial class RestrictableTableEditorManager : DockPanel
    {
        /// <summary>
        /// SqliteConnector
        /// </summary>
        private readonly SqliteConnector _con;

        /// <summary>
        /// DataTable
        /// </summary>
        private DataTable _tables;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con">SqliteConnector</param>
        public RestrictableTableEditorManager(SqliteConnector con)
        {
            _con = con;
            InitializeComponent();
            PopulateListView();
        }

        /// <summary>
        /// Fills the ListView
        /// </summary>
        private void PopulateListView()
        {
            _con.Query("Select * from RestrictedTableEditorPermissions", out DataTable dt);
            _tables = dt;
            foreach (DataRow dr in dt.Rows)
            {
                TableListView.Items.Add(new TablePermissionWrap((string) dr[1].ToString(), dr));
            }
        }

        /// <summary>
        /// CheckBox Check/Uncheck Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxPermissionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox) sender;
            if (comboBox.Items.Count == 3)
            {
                var content = (ComboBoxDataRowKeyStringValueWrap) comboBox.SelectedItem;
                UpdateTable((long) content.Row[0],
                    (int) Enum.Parse(typeof(RestrictableTableEditor.AccessType), content.Value));
            }
        }

        /// <summary>
        /// Updates the DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        private void UpdateTable(long id, int permission)
        {
            _con.NonQuery(
                "Update RestrictedTableEditorPermissions set Permission = " + permission + " where Id = " + id);
        }

        /// <summary>
        /// Wrapper for a ListView item
        /// </summary>
        internal class TablePermissionWrap
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="tableName">tableName</param>
            /// <param name="dr">DataRow</param>
            public TablePermissionWrap(string tableName, DataRow dr)
            {
                TableName = tableName;
                Permission = new List<ComboBoxDataRowKeyStringValueWrap>()
                {
                    new ComboBoxDataRowKeyStringValueWrap(dr, RestrictableTableEditor.AccessType.NoAccess.ToString()),
                    new ComboBoxDataRowKeyStringValueWrap(dr, RestrictableTableEditor.AccessType.Ro.ToString()),
                    new ComboBoxDataRowKeyStringValueWrap(dr, RestrictableTableEditor.AccessType.Rw.ToString())
                };
            }

            public string TableName { get; }
            public List<ComboBoxDataRowKeyStringValueWrap> Permission { get; }
        }

        /// <summary>
        /// OnloadEvent for the Combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var cBox = (ComboBox) sender;
            var item = (ComboBoxDataRowKeyStringValueWrap) cBox.Items[0];
            cBox.SelectedIndex = (int) item.Row[2];
        }
    }
}
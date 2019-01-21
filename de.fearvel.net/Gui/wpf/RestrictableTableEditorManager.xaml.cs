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
    /// </summary>
    public partial class RestrictableTableEditorManager : DockPanel
    {
        private readonly SqliteConnector _con;
        private DataTable _tables;

        public RestrictableTableEditorManager(SqliteConnector con)
        {
            _con = con;
            InitializeComponent();
            PopulateListView();
        }


        private void PopulateListView()
        {
            _con.Query("Select * from RestrictedTableEditorPermissions", out DataTable dt);
            _tables = dt;
            foreach (DataRow dr in dt.Rows)
            {
                TableListView.Items.Add(new TablePermissionWrap((string)dr[1].ToString(), dr));
            }
            

        }

        private void ComboBoxPermissionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox) sender;
            if (comboBox.Items.Count == 3)
            {
                var content = (ComboBoxKeyValueItem)comboBox.SelectedItem;
                UpdateTable((long)content.Row[0], (int)Enum.Parse(typeof(RestrictableTableEditor.AccessType), content.Value));
            }
           }

        private void UpdateTable(long id, int permission)
        {
            _con.NonQuery(
                "Update RestrictedTableEditorPermissions set Permission = " + permission + " where Id = " + id);
        }


        internal class TablePermissionWrap
        {
            public TablePermissionWrap(string tableName, DataRow dr)
            {
                TableName = tableName;
                Permission = new List<ComboBoxKeyValueItem>()
                {
                    new ComboBoxKeyValueItem(dr, RestrictableTableEditor.AccessType.NoAccess.ToString()),
                    new ComboBoxKeyValueItem(dr, RestrictableTableEditor.AccessType.Ro.ToString()),
                    new ComboBoxKeyValueItem(dr, RestrictableTableEditor.AccessType.Rw.ToString())
                };
            }

            public string TableName { get; }
            public List<ComboBoxKeyValueItem> Permission { get; }
        }

        private void ContentComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var cBox = (ComboBox) sender;
            var item =  (ComboBoxKeyValueItem)cBox.Items[0];
            cBox.SelectedIndex = (int) item.Row[2];

        }
    }
}
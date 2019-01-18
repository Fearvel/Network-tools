using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using de.fearvel.net.SQL.Connector;
using DataTable = System.Data.DataTable;

namespace de.fearvel.net.Gui.wpf
{
    public class RestrictableTableEditor
    {
        private static RestrictableTableEditor _instance;
        private SqliteConnector _dbConnection;
        private SQLiteDataAdapter _da;
        private DataSet _ds;
        private bool _rw = false;

        public DockPanel TableEditor { get; private set; }
        private ComboBox _tableSelection;
        private DataGrid _editDataGrid;


        public static void SetInstance(SqliteConnector con)
        {
            _instance = new RestrictableTableEditor(con);
        }


        public void GetTables()
        {
            DataTable dt = null;
            var cmd = "SELECT name FROM sqlite_master" +
                      " WHERE type = 'table' and" +
                      " Name not like 'sqlite_sequence' and" +
                      " Name not like 'RestrictedTableEditorPermissionDefinition' and" +
                      " Name not like 'RestrictedTableEditorPermissions' and" +
                      " Name not in (Select TableName from RestrictedTableEditorPermissions);";
            _dbConnection.Query(cmd, out dt);
            foreach (DataRow row in dt.Rows)
            {
                _dbConnection.NonQuery("Insert into RestrictedTableEditorPermissions (TableName) values ('" +
                                       row.Field<string>("name") + "')");
            }

            _dbConnection.NonQuery("delete from RestrictedTableEditorPermissions " +
                                   "where TableName not in (SELECT name FROM sqlite_master WHERE type = 'table')"); //Clean
        }

        public void CreateTables()
        {
            _dbConnection.NonQuery("CREATE TABLE IF NOT EXISTS RestrictedTableEditorPermissionDefinition" +
                                   " (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                   " Name varchar(200) NOT NULL)");
            _dbConnection.Query("Select * from RestrictedTableEditorPermissionDefinition", out DataTable dt);
            if (dt.Rows.Count == 0)
            {
                _dbConnection.NonQuery(
                    "Insert into RestrictedTableEditorPermissionDefinition (Name) Values ('No Access'), ('RO'),('RW')");
            }

            _dbConnection.NonQuery("CREATE TABLE IF NOT EXISTS RestrictedTableEditorPermissions " +
                                   "(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                   "TableName text NOT NULL, " +
                                   "Permission int not null default 1, " +
                                   "FOREIGN KEY(Permission) REFERENCES RestrictedTableEditorPermissionDefinition(Id))");
        }

        public static RestrictableTableEditor GetInstance()
        {
            return _instance;
        }

        private RestrictableTableEditor(SqliteConnector con)
        {
            _dbConnection = con;
            CreateTables();
            GetTables();

            _editDataGrid = new DataGrid();
            TableEditor = new DockPanel()
            {
                Background = Brushes.White
            };

            _tableSelection = new ComboBox();
         
            _editDataGrid.SelectedCellsChanged += new SelectedCellsChangedEventHandler(DataGridOnSelectedCellsChanged);
            _tableSelection.SelectionChanged += new SelectionChangedEventHandler(TableSelectionOnSelectionChanged);
            UpdateTableSelection();
            BuildStructure();
        }


        private void UpdateTableSelection()
        {
            _dbConnection.Query("Select * from RestrictedTableEditorPermissions where Permission > 1",
                out DataTable dt);
            foreach (DataRow dr in dt.Rows)
            {
                _tableSelection.Items.Add(dr[1]);
            }
        }

        private void TableSelectionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tableSelection.SelectedIndex >= 0)
            {
                DataTable dtTemp;
                _dbConnection.Query(
                    "Select * from RestrictedTableEditorPermissions where TableName ='" +
                    (string)_tableSelection.SelectedValue + "' and Permission > 1", out dtTemp);
                if (dtTemp.Rows.Count == 1)
                {
                    _rw = (int)dtTemp.Rows[0][2] == 3;
                    _editDataGrid.IsReadOnly = !_rw;
                    _editDataGrid.CanUserAddRows = _rw;


                    _ds = new DataSet();
                    _da = _dbConnection.Query("Select * from " + (string)_tableSelection.SelectedValue);
                    _da.Fill(_ds);
                    _editDataGrid.ItemsSource = _ds.Tables[0].DefaultView;
                }
            }
        }

       

        private void DataGridOnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (_rw && _ds.GetChanges() != null)
            {
                _dbConnection.Update(_da, _ds);
                                _ds.Clear();
                _da.Fill(_ds);
            }
        }

        private void BuildStructure()
        {
            _tableSelection.Margin = new Thickness(10, 10, 0, 0);
            _tableSelection.Width = 200;
            _tableSelection.Height = 28;

            _editDataGrid.Margin = new Thickness(10, 10, 10, 10);


            var sp = new StackPanel()
            {
                Background = Brushes.White,
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Label()
                    {
                        Content = "Table:",
                        Margin = new Thickness(20, 10, 0, 0)
                    },
                    _tableSelection
                    
                }
            };
            DockPanel.SetDock(sp, Dock.Top);
            TableEditor.Children.Add(sp);
            TableEditor.Children.Add(_editDataGrid);
        }
    }
}
using System;
using de.fearvel.net.SQL.Connector;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using de.fearvel.net.DataTypes.Exceptions;
using DataTable = System.Data.DataTable;


namespace de.fearvel.net.Gui.wpf
{
    /// <summary>
    /// A simple way to edit or view a used Sqlite database encrypted or not.
    /// This will create 1 Table RestrictedTableEditorPermissions.
    /// RestrictedTableEditorPermissions will be automatically filled with the names of the existing tables.
    /// It automatically sets the access to the value 0 which is normally no access.
    /// It also removes deprecated entries on load.
    /// To Allow the User to access some Tables their permissions need to be set to a prior defined value different than 0.
    /// </summary>
    public class RestrictableTableEditor
    {
        public enum AccessType : int
        {
            NoAccess,
            Ro,
            Rw
        };
        public static AccessType ParseAccessType(int i) => (AccessType)i;



        private static RestrictableTableEditor _instance;
        private readonly SqliteConnector _dbConnection;
        private SQLiteDataAdapter _da;
        private DataSet _ds;
        private bool _rw = false;

        public DockPanel TableEditor { get; private set; }
        private readonly ComboBox _tableSelection;
        private readonly DataGrid _editDataGrid;

        /// <summary>
        /// Sets the instance
        /// Without this GetInstance it will throw an InstanceNotSetException
        /// </summary>
        /// <param name="con"></param>
        public static void SetInstance(SqliteConnector con)
        {
            _instance = new RestrictableTableEditor(con);
        }

        /// <summary>
        /// UpdateRestrictedTableEditorPermissionsEntries
        /// Inserts missing table names into RestrictedTableEditorPermissions
        /// deletes obsolete table names from RestrictedTableEditorPermissions
        /// </summary>
        public void UpdateRestrictedTableEditorPermissionsEntries()
        {
            const string cmd = "SELECT name FROM sqlite_master" +
                               " WHERE type = 'table' and" +
                               " Name not like 'sqlite_sequence' and" +
                               " Name not like 'RestrictedTableEditorPermissions' and" +
                               " Name not in (Select TableName from RestrictedTableEditorPermissions);";
            _dbConnection.Query(cmd, out DataTable dt);
            foreach (DataRow row in dt.Rows)
            {
                _dbConnection.NonQuery("Insert into RestrictedTableEditorPermissions (TableName) values ('" +
                                       row.Field<string>("name") + "')");
            }

            _dbConnection.NonQuery("delete from RestrictedTableEditorPermissions " +
                                   "where TableName not in (SELECT name FROM sqlite_master WHERE type = 'table')"); //Clean
        }

        /// <summary>
        /// Creates the RestrictedTableEditorPermissions tables
        /// </summary>
        public void CreateTables()
        {
            _dbConnection.NonQuery("CREATE TABLE IF NOT EXISTS RestrictedTableEditorPermissions " +
                                   "(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                   "TableName text NOT NULL, " +
                                   "Permission int not null default 0)");
        }

        /// <summary>
        /// Gets the prior set instance.
        /// </summary>
        /// <returns></returns>
        public static RestrictableTableEditor GetInstance()
        {
            return _instance != null ? _instance : throw new InstanceNotSetException();
        }

        /// <summary>
        /// Constructor
        /// Builds the Gui elements.
        /// Creates Missing Tables.
        /// Triggers an update of RestrictedTableEditorPermissions.
        /// It also adds the needed events to some Controls.
        /// </summary>
        /// <param name="con"></param>
        private RestrictableTableEditor(SqliteConnector con)
        {
            _dbConnection = con;
            CreateTables();
            UpdateRestrictedTableEditorPermissionsEntries();

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

        /// <summary>
        /// UpdateTableSelection()
        /// Fills the Combobox with Table names on which the user has at least read permission.
        /// </summary>
        private void UpdateTableSelection()
        {
            _dbConnection.Query("Select * from RestrictedTableEditorPermissions where Permission > 0",
                out DataTable dt);
            foreach (DataRow dr in dt.Rows)
            {
                _tableSelection.Items.Add(dr[1]);
            }
        }


        /// <summary>
        /// TableSelectionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        /// Event of the Combobox to display the content of an selected Table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableSelectionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tableSelection.SelectedIndex >= 0)
            {
                _dbConnection.Query(
                    "Select * from RestrictedTableEditorPermissions where TableName ='" +
                    (string) _tableSelection.SelectedValue + "' and Permission > 0", out DataTable dtTemp);
                if (dtTemp.Rows.Count == 1)
                {
                    _rw = (int) dtTemp.Rows[0][2] == 2;
                    _editDataGrid.IsReadOnly = !_rw;
                    _editDataGrid.CanUserAddRows = _rw;


                    _ds = new DataSet();
                    _da = _dbConnection.Query("Select * from " + (string) _tableSelection.SelectedValue);
                    _da.Fill(_ds);
                    _editDataGrid.ItemsSource = _ds.Tables[0].DefaultView;
                }
            }
        }

        /// <summary>
        /// DataGridOnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        /// Event of the DataGrid.
        /// Updates Changed Cells in the Sqlite Database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridOnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!_rw || _ds.GetChanges() == null) return;
            _dbConnection.Update(_da, _ds);
            _ds.Clear();
            _da.Fill(_ds);
        }
       
        /// <summary>
        /// Sets values of basic gui elements.
        /// </summary>
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
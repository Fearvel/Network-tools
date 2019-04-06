using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace de.fearvel.net.Gui.wpf
{
    /// <summary>
    /// Interaktionslogik für SetManager.xaml
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public partial class SetManager : UserControl
    {
        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        private readonly SqlDataAdapter _sqlDataAdapter;

        /// <summary>
        /// DataSet
        /// </summary>
        private readonly DataSet _dataSet = new DataSet();

        /// <summary>
        /// ListViewColumn
        /// </summary>
        private readonly string _listViewColumn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="con">SqlDataAdapter</param>
        /// <param name="listViewColumn">listViewColumn</param>
        public SetManager(SqlDataAdapter con, string listViewColumn)
        {
            InitializeComponent();
            _sqlDataAdapter = con;
            _listViewColumn = listViewColumn;
            _sqlDataAdapter.Fill(_dataSet);

            GenerateAndDisplayListView();
        }

        /// <summary>
        /// Generates List view
        /// </summary>
        /// <param name="filter">filterString</param>
        private void GenerateAndDisplayListView(string filter = "")
        {
            DataTable dt = _dataSet.Tables[0];
            if (filter.Length > 0)
            {
                var q = dt.AsEnumerable()
                    .Where(r => r.Field<string>(_listViewColumn).ToLower().Contains(filter.ToLower()));

                if (q.Any())
                {
                    var a = q.CopyToDataTable();
                    ListViewSetItems.ItemsSource = GenerateListFromDataTable(a);
                }
                else
                {
                    ListViewSetItems.ItemsSource = null;
                }
            }
            else
            {
                ListViewSetItems.ItemsSource = GenerateListFromDataTable(dt);
            }
        }

        /// <summary>
        /// List of string to compatible DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<string> GenerateListFromDataTable(DataTable dt)
        {
            var list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row.Field<string>(_listViewColumn));
            }

            return list;
        }

        /// <summary>
        /// TextBoxSearch_OnTextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextboxSeach.Text.Length > 0)
            {
                GenerateAndDisplayListView(TextboxSeach.Text);
            }
            else
            {
                GenerateAndDisplayListView();
            }
        }

        /// <summary>
        /// ButtonAdd_OnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// ListViewSetItems_OnSelectionChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewSetItems_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
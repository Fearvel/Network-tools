using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
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
using de.fearvel.net;
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.SocketIo;
using de.fearvel.net.FnLog;
using de.fearvel.net.Gui.wpf;
using de.fearvel.net.SQL.Connector;


namespace WpfTester
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                SqliteConnector con = new SqliteConnector(@"C:\Users\schreiner.andreas\test2.db");

                DataSet ds = new DataSet();
                var da = con.Query("Select * from aaa");
                da.Fill(ds);

                ds.Tables[0].Rows[0][1] = "newValb";
                
                con.Update(da, ds);
                ds.Tables[0].Rows[0][1] = "newValc";
                con.Update(da, ds);
                ds.Tables[0].Rows[0][1] = "newVald";
                con.Update(da, ds);

                de.fearvel.net.Gui.wpf.RestrictableTableEditor.SetInstance(con);
                MainGrid.Children.Add(de.fearvel.net.Gui.wpf.RestrictableTableEditor.GetInstance().TableEditor);
                RestrictableTableEditor.GetInstance().GetTables();

            }
            catch (AccessKeyDeclinedException e)
            {
                Console.WriteLine(e);
            }
        }


        public DockPanel CreateLimitedTableEditor()
        {
            var dp = new DockPanel();

            var sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Label()
                    {
                        Content = "Table:",
                        Margin = new Thickness(20, 20, 0, 0)
                    },
                    new ComboBox()
                    {
                        Name = "TableComboBox",
                        Width = 200,
                        Margin = new Thickness(20, 20, 0, 0)
                    },
                    new Button()
                    {
                        Content = "Open",
                        Width = 100,
                        Margin = new Thickness(20, 20, 0, 0)
                    }
                }
            };
            DockPanel.SetDock(sp,Dock.Top);
            var dg = new DataGrid();


            dp.Children.Add(sp);
            dp.Children.Add(dg);

            return dp;
        }

        private TabControl GetDbGroupBox()
        {
            var tabControl = new TabControl();
            tabControl.Items.Add(new TabItem() {Header = "AAAAA"});
            return tabControl;
        }

        
        }
}
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
               // SqliteConnector con = new SqliteConnector(@"C:\Users\schreiner.andreas\test2.db");
                //
                //DataSet ds = new DataSet();
                //var da = con.Query("Select * from aaa");
                //da.Fill(ds);
                //
                //ds.Tables[0].Rows[0][1] = "newValb";
                //
                //con.Update(da, ds);
                //ds.Tables[0].Rows[0][1] = "newValc";
                //con.Update(da, ds);
                //ds.Tables[0].Rows[0][1] = "newVald";
                //con.Update(da, ds);
                //
                //de.fearvel.net.Gui.wpf.RestrictableTableEditor.SetInstance(con);
                //MainGrid.Children.Add(de.fearvel.net.Gui.wpf.RestrictableTableEditor.GetInstance().TableEditor);
                //RestrictableTableEditor.GetInstance().GetTables();
                //FnLog.SetInstance(new FnLog.FnLogInitPackage("","TESTER",Version.Parse("0.0.0.1"),FnLog.TelemetryType.LogLocalOnly,"test.db",""));
                //var r =  new Random();
                //for (int i = 0; i < 20; i++)
                //{
                //    FnLog.GetInstance().Log(FnLog.LogType.CriticalError,r.Next(99).ToString(), r.Next(99).ToString());
                //}
                //
                //MainGrid.Children.Add(FnLog.GetInstance().GetViewer().FnLogTable);
             //  de.fearvel.net.Gui.wpf.RestrictableTableEditor.SetInstance(con);
             //
             //  MainGrid.Children.Add(new RestrictableTableEditorManager(con));

                //var con = new MssqlConnector("FN-MSSQL", "SaphireOffice");
                //var adapter = con.Query("Select * from [SaphireOffice].[Address].[Salutation]");
                //var sm = new SetManager(adapter, "Salutation");
                //MainGrid.Children.Add(sm);

               // var test = new SearchTextBox();
               // MainGrid.Children.Add(test);
            }
            catch (AccessKeyDeclinedException e)
            {
                Console.WriteLine(e);
            }
        }

        public class TestWrap

        {
            public int Id { get; private set; }
            public string TableName { get; private set; }
            public bool NoAccess { get; private set; }
            public ComboBox AccessLevel { get; private set; }

            public TestWrap(int id, string tableName, int accessLevel)
            {
                AccessLevel = new ComboBox();
                AccessLevel.Items.Add("No Access");
                AccessLevel.Items.Add("ReadOnly");
                AccessLevel.Items.Add("ReadWrite");
                Id = id;
                TableName = tableName;
                AccessLevel.SelectedIndex = accessLevel;
                if (accessLevel == 0)
                {
                    NoAccess = true;
                }
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
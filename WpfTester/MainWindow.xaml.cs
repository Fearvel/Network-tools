using System;
using System.Collections.Generic;
using System.Data;
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
using de.fearvel.net.FnLog;
using de.fearvel.net.Exceptions;


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
            //FPing fp = new FPing(new IPAddress(new byte[] { 192, 168, 1, 1 }), new IPAddress(new byte[] { 192, 168, 1, 211 }));        
            //FPingResult ips = fp.RangePing();
            //

            //foreach (var ip in ips.SuccessIpAddresses)
            //{
            //    lbox.Items.Add(ip);
            //}
            try
            {

               
                
                //var dt = de.fearvel.net.SocketIo.SocketIoClient.RetrieveSingleValue<DataTable>("https://localhost:9051",
                //    "oidTable", "oid", new ValueWrap() { Val = "aaaaaa" }.Serialize());
                //DataGrid.ItemsSource = dt.DefaultView;

            }
            catch (AccessKeyDeclinedException e)
            {
                Console.WriteLine(e);
            }
            
            
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            DataGrid.ItemsSource = (await FnLogClient.RetrieveLogsAsync("https://localhost:9024", new ValueWrap() { Val = "aaaaaa" }, true)).DefaultView;

        }
    }
}

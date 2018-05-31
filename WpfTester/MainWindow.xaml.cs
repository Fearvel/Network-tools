using System;
using System.Collections.Generic;
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

namespace WpfTester
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            FPing fp = new FPing(new IPAddress(new byte[] { 192, 168, 1, 1 }), new IPAddress(new byte[] { 192, 168, 1, 211 }));        
            FPingResult ips = fp.RangePing();
            InitializeComponent();

            foreach (var ip in ips.SuccessIpAddresses)
            {
                lbox.Items.Add(ip);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net;
using de.fearvel.net.Ping;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            FPing fp = new FPing(new IPAddress(new byte[] { 192, 168, 1, 1 }), new IPAddress(new byte[] { 192, 168, 1, 211 }));
            TimeSpan est = fp.CalculateEstimatedTime();
            FPingResult ips = fp.RangePing();
            foreach (var ip in ips.SuccessIpAddresses)
            {
                Console.Out.WriteLine(ip.ToString());
            }

            Console.ReadLine();
        }
    }
}

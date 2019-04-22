using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes;
using de.fearvel.net.FnLog;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using de.fearvel.net.DataTypes.SocketIo;
using de.fearvel.net.DataTypes.Manastone;
using System.Security.Cryptography;
using de.fearvel.net.DataTypes.FnLog;
using de.fearvel.net.Manastone;
using de.fearvel.net.SocketIo;
using de.fearvel.net.Security;

namespace ConsoleLogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // ManastoneClient.SetInstance("https://localhost:9060", "00000000-0000-0000-0000-000000000000",ManastoneClient.LicenseCheckType.Online);
            // //ManastoneClient.GetInstance().Activate("ebdd0116-620d-11e9-b74a-000c2910963e");
            // var l = ManastoneClient.GetInstance().CheckActivation();
            // var v = ManastoneClient.GetInstance().CheckToken();
            //
            // var e = ManastoneClient.GetInstance().CustomerReference;
            // var f = ManastoneClient.GetInstance().ManastoneServerVersion;
            // var l = new List<ActivationOffer>();
            //  var j = JsonConvert.SerializeObject(l, Formatting.Indented).Trim().
            //      Replace(System.Environment.NewLine, "");
            //
            var ver = SocketIoClient.RetrieveSingleValue<VersionWrapper>("https://localhost:9051",
                "MPSMinClientVersionOffer", "MPSMinClientVersionRequest", null);
            var ab = Ident.GetFileVersion();
            var ac =      System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var a = 1; //DEBUG

            



        }



        
            
        
    }
}
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

namespace ConsoleLogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ManastoneClient.SetInstance("https://localhost:9060", "00000000-0000-0000-0000-000000000000",ManastoneClient.LicenseCheckType.Mixed);
            //ManastoneClient.GetInstance().Activate("ebdd0116-620d-11e9-b74a-000c2910963e");
            //var e = ManastoneClient.GetInstance().CustomerReference;
            //  var c = ManastoneClient.GetInstance()
            //    .CheckLicenseStatusOnline(new ActivationOnlineCheckRequest("810c2124-6143-11e9-b74a-000c2910963e"));
            //ManastoneClient.GetInstance().RetrieveCustomerReference(new CustomerReferenceRequest("810c2124-6143-11e9-b74a-000c2910963e"));
            var v = ManastoneClient.GetInstance().CheckToken();
            var l = ManastoneClient.GetInstance().CheckActivation();
            var f = ManastoneClient.GetInstance().ManastoneServerVersion;


            var a = 1; //DEBUG
        }
    }
}
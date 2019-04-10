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
using de.fearvel.net.SocketIo;

namespace ConsoleLogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //      FnLog.SetInstance(
            //          new FnLog.FnLogInitPackage("https://localhost:9020", "TESTERV",new Version(1,1,1,1),FnLog.TelemetryType.LogLocalAndSendErrorsAndWarnings,"fnlog.db","") 
            //         );
            //      FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            //
            //      LogRequest lr = new LogRequest("AAAAAAAAAAAAAAAAA", "00000000-0000-0000-0000-000000000000");
            //   var ls =    FnLogClient.RetrieveLog(lr, "https://localhost:9020");
            //
            //   var i = 1;

               var a =  SocketIoClient.RetrieveSingleValue<VersionWrapper>("https://localhost:9051", "OidVersion", "OidVersionRequest",null);

            var i = 1;



            // de.fearvel.net.SocketIo.SocketIoClient.RetrieveSingleValue<SimpleResult>(@"https://localhost:9020/", "put", "post", "aasd", true);


            //FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            //FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            //FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            //FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            //FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");

            //var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            //ManagementObjectCollection mbsList = mbs.Get();
            //string id = "";
            //foreach (ManagementObject mo in mbsList)
            //{
            //    id = mo["ProcessorId"].ToString();
            //    break;
            //}
            //Console.Out.WriteLine(id);

            //  var eqw = de.fearvel.net.SocketIo.SocketIoClient
            //  .RetrieveSingleValue<OfferWrapper<ManastoneActivationOffer>>(
            //  "https://127.0.0.1:9041", "ActivationOffer", "ActivationRequest", new ManastoneActivationRequest("2131212").Serialize());

            //de.fearvel.net.Manastone.ManastoneController.SetInstance("aaaaaa",Guid.NewGuid().ToString());
            //de.fearvel.net.Manastone.ManastoneController.GetInstance().RemoveEncryption();
            //OfferWrapper<ManastoneActivationOffer> o = new OfferWrapper<ManastoneActivationOffer>(
            //    new ManastoneActivationOffer("123213", DateTime.Now)
            //    , new SimpleResult(true));
            //
            // //  var a = o.Serialize();
            // var json =
            //     "{\r\n  \"Data\": {\r\n    \"ActivationKey\": \"bbbbb\",\r\n    \"ValidUntil\": \"2018-11-3T22:20:2.900+01:00\"\r\n  },\r\n  \"Result\": {\r\n    \"Result\": true\r\n  }\r\n}";
            // OfferWrapper<ManastoneActivationOffer> offer = OfferWrapper<ManastoneActivationOffer>.DeSerialize(json);

            // byte[] bytes = Encoding.Unicode.GetBytes("aaaaa");
            //
            // Console.WriteLine(SHA256HexHashString("aaaaa"));

            //  var b = ManastoneActivationWrap.DecryptAndDeserialize(
            //      "ybs+GXt6ekTWHIuLAqpB1zJUQN7ee2PizqTZSzoAyrN7N//3GkdT0ZD1B60UxY97FBkOUlHOQQc6xNDx3ctdd6s7xr+OBKez",
            //      "abcdefghijklmnop", "2e484407");
        }


        //private static string ToHex(byte[] bytes, bool upperCase)
        //{
        //    StringBuilder result = new StringBuilder(bytes.Length * 2);
        //    for (int i = 0; i < bytes.Length; i++)
        //        result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
        //    return result.ToString();
        //}
        //
        //private static string SHA256HexHashString(string StringIn)
        //{
        //    string hashString;
        //    using (var sha256 = SHA256Managed.Create())
        //    {
        //        var hash = sha256.ComputeHash(Encoding.Default.GetBytes(StringIn));
        //        hashString = ToHex(hash, true);
        //    }
        //
        //    return hashString;
        //}
    }
}
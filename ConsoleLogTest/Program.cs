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
namespace ConsoleLogTest
{
    class Program
    {
        static void Main(string[] args)
        {

            // FnLog.SetInstance(
            //     new FnLog.FnLogInitPackage("https://192.168.1.60:9024", "TESTERV",new Version(1,1,1,1),FnLog.TelemetryType.LogLocalAndSendErrorsAndWarnings,"fnlog.db","") 
            //    );
            // FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            // FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            // FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            // FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            // FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            // FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");

            //var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            //ManagementObjectCollection mbsList = mbs.Get();
            //string id = "";
            //foreach (ManagementObject mo in mbsList)
            //{
            //    id = mo["ProcessorId"].ToString();
            //    break;
            //}
            //Console.Out.WriteLine(id);

            var eqw = de.fearvel.net.SocketIo.SocketIoClient
            .RetrieveSingleValue<de.fearvel.net.DataTypes.SocketIo.OfferWrapper<ManastoneActivationOffer>>(
            "https://127.0.0.1:9024", "ActivationOffer", "ActivationRequest", new ManastoneActivationRequest("2131212").Serialize());

            // OfferWrapper<ManastoneActivationOffer> o = new OfferWrapper<ManastoneActivationOffer>(
            //     new ManastoneActivationOffer("123213",DateTime.Now)
            //     , new SimpleResult(true));

            //  var a = o.Serialize();
            var json =
                "{\r\n  \"Data\": {\r\n    \"ActivationKey\": \"bbbbb\",\r\n    \"ValidUntil\": \"2018-11-3T22:20:2.900+01:00\"\r\n  },\r\n  \"Result\": {\r\n    \"Result\": true\r\n  }\r\n}";
            OfferWrapper<ManastoneActivationOffer> offer = OfferWrapper<ManastoneActivationOffer>.DeSerialize(json);
        }


    
    }

}

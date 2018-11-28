using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes;
using de.fearvel.net.FnLog;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;

namespace ConsoleLogTest
{
    class Program
    {
        static void Main(string[] args)
        {


            FnLog.SetInstance(
                new FnLogInitPackage("https://log.fearvel.de:9024", "TESTERV",new Version(1,1,1,1),FnLog.TelemetryType.LogLocalAndSendErrorsAndWarnings,"fnlog.db","") 
               );
            FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");
            FnLog.GetInstance().Log(FnLog.LogType.CriticalError, "ProgramInfoeee", "Program Started");




        }
    }

}

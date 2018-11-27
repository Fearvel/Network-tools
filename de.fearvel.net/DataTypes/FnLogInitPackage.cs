using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using Newtonsoft.Json;
using static de.fearvel.net.FnLog.FnLog;

namespace de.fearvel.net.DataTypes
{
    public class FnLogInitPackage : JsonSerializable<FnLogInitPackage>
    {
        public string LogServer;
        public string ProgramName;
        public Version ProgramVersion;
        public TelemetryType Telemetry;
        public string FileName;
        public string EncryptionKey;

        public FnLogInitPackage(string logServer, string programName,
            Version programVersion, TelemetryType telemetry,
            string fileName, string encryptionKey)
        {
            LogServer = logServer;
            ProgramName = programName;
            ProgramVersion = programVersion;
            Telemetry = telemetry;
            FileName = fileName;
            EncryptionKey = encryptionKey;
        }
    }
}

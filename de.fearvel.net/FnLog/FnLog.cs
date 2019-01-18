using System;
using System.Data;
using de.fearvel.net.FnLog.Database;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.FnLog
{
    public class FnLog
    {
        public static Version FnLogClientVersion => Version.Parse("2.0.1.0");
        private static FnLog _instance;
        public static bool IsInitialized => _instance != null;
        private readonly FnLogInitPackage _fnLogInitPackage;
        private static LocalLogger _localLog;

        public enum LogType : int
        {
            CriticalError,
            Error,
            Warning,
            Notice,
            RuntimeInfo
        };

        public enum TelemetryType : int
        {
            LogLocalOnly,
            LogLocalAndSendErrorsAndWarnings,
            LogLocalSendAll
        }

        public static void SetInstance(FnLogInitPackage fip)
        {
            _instance = new FnLog(fip);
        }

        public static void SetInstance(FnLogInitPackage fip, SqliteConnector con)
        {
            _instance = new FnLog(fip, con);
        }

        public static FnLog GetInstance()
        {
            if (_instance == null)
                throw new InstanceNotSetException("Please Set Instance before Getting it");
            return _instance;
        }

        public LogType ParseLogType(int i) => (LogType) i;


        protected FnLog(FnLogInitPackage fip)
        {
            _fnLogInitPackage = fip;
            _localLog = new LocalLogger(
                _fnLogInitPackage.FileName, _fnLogInitPackage.EncryptionKey);
        }

        protected FnLog(FnLogInitPackage fip, SqliteConnector con)
        {
            _fnLogInitPackage = fip;
            fip.FileName = "";
            _localLog = new LocalLogger(con);
        }


        public void Log(LogType t, string title, string description)
        {
            try
            {
                if (_fnLogInitPackage.Telemetry != TelemetryType.LogLocalOnly)
                {
                    if ((
                            t == LogType.CriticalError ||
                            t == LogType.Error ||
                            t == LogType.Warning) ||
                        _fnLogInitPackage.Telemetry == TelemetryType.LogLocalSendAll
                    )
                    {
                        _localLog.AddLog(t, title, description, true);
                        FnLogClient.SendLog(new LogWrap()
                        {
                            ProgramName = _fnLogInitPackage.ProgramName,
                            FnLogVersion = FnLogClientVersion.ToString(),
                            ProgramVersion = _fnLogInitPackage.ProgramVersion.ToString(),
                            Guid = _localLog.Guid.ToString(),
                            LogType = (int) t,
                            Title = title,
                            Description = description
                        }, _fnLogInitPackage.LogServer);
                    }
                }
                else _localLog.AddLog(t, title, description);
            }
            catch (Exception e)
            {
                _localLog.AddLog(LogType.Error, "SimpleLogSenderException", e.Message + e.StackTrace);
            }
        }

        public DataTable GetLog(LogType t) =>
            _localLog.GetLog(t);

        public DataTable GetErrorsAndWarnings() =>
            _localLog.GetErrorsAndWarnings();


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

        public class LogWrap : JsonSerializable<LogWrap>
        {
            public string ProgramName;
            public string ProgramVersion;
            public string FnLogVersion;
            public string Title;
            public string Description;
            public int LogType;
            public string Guid;
        }
    }
}
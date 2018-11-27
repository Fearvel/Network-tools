using System;
using System.Data;
using de.fearvel.net.FnLog.Database;
using de.fearvel.net.DataTypes;
using de.fearvel.net.Exceptions;
namespace de.fearvel.net.FnLog
{

    public class FnLog
    {
        public static Version FnLogClientVersion => Version.Parse("2.0.0.0");
        private static FnLog _instance;
        private readonly FnLogInitPackage _fnLogInitPackage;

        private static FnLogClient _logClient;
        private static LocalLogger _localLog;
        public enum LogType : int { CriticalError, Error, Warning, Notice, RuntimeInfo };
        public enum TelemetryType : int { LogLocalOnly, LogLocalAndSendErrorsAndWarnings, LogLocalSendAll }

        public static void SetInstance(FnLogInitPackage fip)
        {
            _instance = new FnLog(fip);
        }

        public static FnLog GetInstance()
        {
            if (_instance == null)
                throw new InstanceNotSetException("Please Set Instance before Getting it");
            return _instance;
        }

        public LogType ParseLogType(int i) => (LogType)i;


        protected FnLog(FnLogInitPackage fip)
        {
            _fnLogInitPackage = fip;
            _localLog = new LocalLogger(
                 _fnLogInitPackage.FileName, _fnLogInitPackage.EncryptionKey);
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
                        if (_logClient == null)
                        {
                            _logClient = new FnLogClient();
                        }
                        _localLog.AddLog(t, title, description, true);
                        _logClient.SendLog(new Log()
                        {
                            ProgramName = _fnLogInitPackage.ProgramName,
                            ProgramVersion = FnLogClientVersion.ToString(),
                            FnLogVersion = _fnLogInitPackage.ProgramVersion.ToString(),
                            Guid = _localLog.Guid.ToString(),
                            LogType = (int)t,
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
    }
}

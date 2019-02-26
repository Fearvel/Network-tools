using System;
using System.Data;
using de.fearvel.net.FnLog.Database;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.Gui.wpf;
using de.fearvel.net.SQL.Connector;

namespace de.fearvel.net.FnLog
{
    /// <summary>
    /// FnLog main Class
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class FnLog
    {
        /// <summary>
        /// Version of FnLog
        /// </summary>
        public static Version FnLogClientVersion => Version.Parse("2.0.2.0");

        /// <summary>
        /// instance for the singleton
        /// </summary>
        private static FnLog _instance;

        /// <summary>
        /// bool true if initialized
        /// </summary>
        public static bool IsInitialized => _instance != null;

        /// <summary>
        /// FnLogInitPackage
        /// </summary>
        private readonly FnLogInitPackage _fnLogInitPackage;

        /// <summary>
        /// LocalLogger
        /// </summary>
        private static LocalLogger _localLog;

        /// <summary>
        /// LogType enum
        /// </summary>
        public enum LogType : int
        {
            CriticalError,
            Error,
            Warning,
            Notice,
            RuntimeInfo
        };

        /// <summary>
        /// TelemetryType enum
        /// </summary>
        public enum TelemetryType : int
        {
            LogLocalOnly,
            LogLocalAndSendErrorsAndWarnings,
            LogLocalSendAll
        }

        /// <summary>
        /// Set Instance for the singleton
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        public static void SetInstance(FnLogInitPackage fip)
        {
            _instance = new FnLog(fip);
        }

        /// <summary>
        /// Set Instance for the singleton
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        /// <param name="con">for integration in a existing sqlite db</param>
        public static void SetInstance(FnLogInitPackage fip, SqliteConnector con)
        {
            _instance = new FnLog(fip, con);
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        /// <returns></returns>
        public static FnLog GetInstance()
        {
            if (_instance == null)
                throw new InstanceNotSetException("Please Set Instance before Getting it");
            return _instance;
        }

        /// <summary>
        /// Parses the LogType from integer
        /// </summary>
        /// <param name="i">int LogType</param>
        /// <returns>enum value of the LogType</returns>
        public static LogType ParseLogType(int i) => (LogType) i;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        protected FnLog(FnLogInitPackage fip)
        {
            _fnLogInitPackage = fip;
            _localLog = new LocalLogger(
                _fnLogInitPackage.FileName, _fnLogInitPackage.EncryptionKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        /// <param name="con">for integration in a existing sqlite db</param>
        protected FnLog(FnLogInitPackage fip, SqliteConnector con)
        {
            _fnLogInitPackage = fip;
            fip.FileName = "";
            _localLog = new LocalLogger(con);
        }

        /// <summary>
        /// writes and or sends a log
        /// </summary>
        /// <param name="t">LogType</param>
        /// <param name="title">title</param>
        /// <param name="description">description</param>
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
                    else
                    {
                        _localLog.AddLog(t, title, description, false);
                    }
                }
                else _localLog.AddLog(t, title, description);
            }
            catch (Exception e)
            {
                _localLog.AddLog(LogType.Error, "SimpleLogSenderException", e.Message + e.StackTrace, false);
            }
        }

        /// <summary>
        /// Gets all logs of a LogType
        /// </summary>
        /// <param name="t">LogType</param>
        /// <returns>DataTable</returns>
        public DataTable GetLog(LogType t) =>
            _localLog.GetLog(t);

        /// <summary>
        /// Gets all Error and Warning Logs
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetErrorsAndWarnings() =>
            _localLog.GetErrorsAndWarnings();

        /// <summary>
        /// Class for init package
        /// </summary>
        public class FnLogInitPackage : JsonSerializable<FnLogInitPackage>
        {
            /// <summary>
            /// LogServer URL
            /// </summary>
            public string LogServer;

            /// <summary>
            /// ProgramName
            /// </summary>
            public string ProgramName;

            /// <summary>
            /// ProgramVersion
            /// </summary>
            public Version ProgramVersion;

            /// <summary>
            /// TelemetryType
            /// </summary>
            public TelemetryType Telemetry;

            /// <summary>
            /// FileName
            /// </summary>
            public string FileName;

            /// <summary>
            /// EncryptionKey
            /// </summary>
            public string EncryptionKey;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="logServer">LogServer URL</param>
            /// <param name="programName">programName</param>
            /// <param name="programVersion">programVersion</param>
            /// <param name="telemetry">TelemetryType</param>
            /// <param name="fileName">fileName</param>
            /// <param name="encryptionKey">encryptionKey</param>
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

        /// <summary>
        /// Wrapper class for FnLog
        /// </summary>
        public class LogWrap : JsonSerializable<LogWrap>
        {
            /// <summary>
            /// ProgramName
            /// </summary>
            public string ProgramName;

            /// <summary>
            /// ProgramVersion
            /// </summary>
            public string ProgramVersion;

            /// <summary>
            /// FnLogVersion
            /// </summary>
            public string FnLogVersion;

            /// <summary>
            /// Title
            /// </summary>
            public string Title;

            /// <summary>
            /// Description
            /// </summary>
            public string Description;

            /// <summary>
            /// LogType as int 
            /// </summary>
            public int LogType;

            /// <summary>
            /// Guid
            /// </summary>
            public string Guid;
        }

        /// <summary>
        /// Returns an FnLogViewer
        /// </summary>
        /// <returns></returns>
        public FnLogViewer GetViewer()
        {
            return new FnLogViewer(_localLog.Connection);
        }
    }
}
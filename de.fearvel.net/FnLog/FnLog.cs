using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.FnLog;
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
        /// Version of FnLog Client
        /// </summary>
        public static Version FnLogClientVersion => Version.Parse("2.000.0005.0000");

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
        /// LocalLogger instance
        /// </summary>
        private readonly LocalLogger _localLog;

        /// <summary>
        /// LogType enum
        /// </summary>
        public enum LogType : int
        {
            CriticalError,
            Error,
            Warning,
            Notice,
            RuntimeInfo,
            MinorRuntimeInfo,
            MajorRuntimeInfo,
            StartupLog,
            MinorStartupLog,
            MajorStartupLog,
            StartupError,
            DrmLog,
            MinorDrmLog,
            MajorDrmLog,
            DrmDatabaseLog,
            DrmError
        };

        /// <summary>
        /// TelemetryType enum
        /// determines if and how much logs will be transmitted
        /// </summary>
        public enum TelemetryType : int
        {
            LogLocalOnly,
            LogLocalAndSendErrorsAndWarnings,
            LogLocalSendAll
        }

        /// <summary>
        /// list of logs to reduce the amount of queries 
        /// </summary>
        private List<Log> _logs;

        /// <summary>
        /// Set Instance for the Singleton
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        public static void SetInstance(FnLogInitPackage fip)
        {
            _instance = new FnLog(fip);
        }

        /// <summary>
        /// Set Instance for the singleton
        /// this will use a existing SqliteConnector
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
        /// creates an empty List of Log
        /// fills properties
        /// creates the LocalLogger instance
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        internal FnLog(FnLogInitPackage fip)
        {
            _logs = new List<Log>();
            _fnLogInitPackage = fip;
            _localLog = new LocalLogger(
                _fnLogInitPackage.FileName, _fnLogInitPackage.EncryptionKey);
        }

        /// <summary>
        /// Constructor used for using an existend SqliteConnection
        /// creates an empty List of Log
        /// fills properties
        /// creates the LocalLogger instance
        /// </summary>
        /// <param name="fip">FnLogInitPackage</param>
        /// <param name="con">for integration in a existing sqlite db</param>
        internal FnLog(FnLogInitPackage fip, SqliteConnector con)
        {
            _logs = new List<Log>();
            _fnLogInitPackage = fip;
            fip.FileName = "";
            _localLog = new LocalLogger(con);
        }

        /// <summary>
        /// Deconstructor
        /// used to write the List of Log to the database and or send it to the Server
        /// </summary>
        ~FnLog()
        {
            ProcessLogList();
        }        

        /// <summary>
        /// writes and or sends a log, depending on the TelemetryType
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
                        _localLog.AddLog(t, title, description);
                        FnLogClient.SendLog(new Log(
                                _fnLogInitPackage.ProgramName,
                                _fnLogInitPackage.ProgramVersion.ToString(),
                                FnLogClientVersion.ToString(),
                                _localLog.UUID.ToString(),
                                title,
                                description,
                                ((int) t))
                            , _fnLogInitPackage.LogServer);
                    }
                    else
                    {
                        _localLog.AddLog(t, title, description);
                    }
                }
                else _localLog.AddLog(t, title, description);
            }
            catch (Exception e)
            {
                _localLog.AddLog(LogType.Error, "SimpleLogSenderException", e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// Adds a log to the logList to reduce the amount of queries
        /// </summary>
        /// <param name="t"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddToLogList(LogType t, string title, string description)
        {
        //    _localLog.AddLog(t, title, description);
            _logs.Add(new Log(_fnLogInitPackage.ProgramName, _fnLogInitPackage.ProgramVersion.ToString(),
                FnLogClientVersion.ToString(), _localLog.UUID.ToString(), title, description, ((int) t)));
            if (_logs.Count == 100)
            {
                ProcessLogList();
            }
        }

        /// <summary>
        /// writes the logList to the internal database
        /// and sends logs to the fnLog server
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ProcessLogList()
        {
            var cloneList = new List<Log>();

            foreach (var log in _logs)
            {
                _localLog.AddLog(log.LogType,log.Title,log.Description);
                cloneList.Add((Log) log.Clone());
            }

            _logs.Clear();

            if (cloneList.Count > 0)
            {
                LogPackage(cloneList);
            }
        }

        /// <summary>
        /// testing!
        /// like ProcessLogList just threaded
        /// </summary>
        /// <returns></returns>
        internal Thread ThreadedProcessLogList()
        {
            return new Thread(ProcessLogList);
        }

        /// <summary>
        /// sends logs list to the server
        /// </summary>
        /// <param name="logs"></param>
        private void LogPackage(List<Log> logs)
        {
            var logsToSend = FilterPackage(logs);
            if (logsToSend.Count > 0)
            {
                FnLogClient.SendLogPackage(logs, _fnLogInitPackage.LogServer);
            }
        }

        /// <summary>
        /// creates a sublist depending on the TelemetryTypeSettings
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        private List<Log> FilterPackage(List<Log> logs)
        {
            var logsToSend = new List<Log>();
            foreach (var log in logs)
            {
                if (_fnLogInitPackage.Telemetry != TelemetryType.LogLocalOnly)
                {
                    if ((
                            log.LogType == (int) LogType.CriticalError ||
                            log.LogType == (int) LogType.Error ||
                            log.LogType == (int) LogType.Warning) ||
                        _fnLogInitPackage.Telemetry == TelemetryType.LogLocalSendAll
                    )
                    {
                        logsToSend.Add(log);
                    }
                }
            }

            return logsToSend;
        }

        /// <summary>
        /// logs a list of log locally in the SQLite DB
        /// </summary>
        /// <param name="logs"></param>
        private void LogPackLocal(List<Log> logs)
        {
            foreach (var log in logs)
            {
                _localLog.AddLog(log.LogType, log.Title, log.Description);
            }
        }

        /// <summary>
        /// Returns a DataTable containing all logs of a LogType
        /// </summary>
        /// <param name="t">LogType</param>
        /// <returns>DataTable</returns>
        public DataTable GetLog(LogType t) =>
            _localLog.GetLog(t);

        /// <summary>
        /// Returns a DataTable containing all logs
        /// </summary>
        /// <param name="t">LogType</param>
        /// <returns>DataTable</returns>
        public DataTable GetLog() =>
            _localLog.GetLog();

        /// <summary>
        /// Returns a DataTable containing all Error and Warning Logs
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetErrorsAndWarnings() =>
            _localLog.GetErrorsAndWarnings();

        /// <summary>
        /// Class for init package
        /// Á Package containing all Information needed to init the FnLog Client
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
            /// fills the Variables with values
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
    }
}
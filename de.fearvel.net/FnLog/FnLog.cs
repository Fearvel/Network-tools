using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.FnLog;
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
        /// LocalLogger
        /// </summary>
        private LocalLogger _localLog;

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
            DrmLog,
            MinorDrmLog,
            MajorDrmLog
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

        private List<Log> _logs;


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
        internal FnLog(FnLogInitPackage fip)
        {
            _logs = new List<Log>();
            _fnLogInitPackage = fip;
            _localLog = new LocalLogger(
                _fnLogInitPackage.FileName, _fnLogInitPackage.EncryptionKey);
        }


        /// <summary>
        /// 
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

        ~FnLog()
        {
            ProcessLogList();
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


        public void AddToLogList(LogType t, string title, string description)
        {
            _localLog.AddLog(t, title, description);
            _logs.Add(new Log(_fnLogInitPackage.ProgramName, _fnLogInitPackage.ProgramVersion.ToString(),
                FnLogClientVersion.ToString(), _localLog.UUID.ToString(), title, description, ((int) t)));
            if (_logs.Count == 40)
            {
                ProcessLogList();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ProcessLogList()
        {
            var cloneList = new List<Log>();

            foreach (var log in _logs)
            {
                cloneList.Add((Log) log.Clone());
            }

            _logs.Clear();

            if (cloneList.Count > 0)
            {
                LogPackage(cloneList);
            }
        }

        private void LogPackage(List<Log> logs)
        {
            List<Log> logsToSend = FilterPackage(logs);
            if (logsToSend.Count > 0)
            {
                FnLogClient.SendLogPackage(logs, _fnLogInitPackage.LogServer);
            }
        }

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

        private void LogPackLocal(List<Log> logs)
        {
            foreach (var log in logs)
            {
                _localLog.AddLog(log.LogType, log.Title, log.Description);
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
    }
}
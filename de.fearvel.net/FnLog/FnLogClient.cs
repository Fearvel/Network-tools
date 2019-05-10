using System.Collections.Generic;
using System.Threading;
using de.fearvel.net.DataTypes.FnLog;
using de.fearvel.net.DataTypes.SocketIo;
using de.fearvel.net.SocketIo;
using Newtonsoft.Json;

namespace de.fearvel.net.FnLog
{
    /// <summary>
    /// FnLogClient
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class FnLogClient
    {
        /// <summary>
        /// ThreadedLogSender
        /// Class for sending a log as a thread       
        /// <copyright>Andreas Schreiner 2019</copyright>
        /// </summary>
        private class ThreadedLogSender
        {
            /// <summary>
            /// FnLog.LogWrap
            /// </summary>
            private readonly string _logSerialized;

            /// <summary>
            /// Server URL
            /// </summary>
            private readonly string _serverUrl;

            /// <summary>
            /// bool == true for accepting self signed certificates
            /// </summary>
            private readonly bool _acceptSelfSigned;

            /// <summary>
            /// timeout in ms
            /// </summary>
            private readonly int _timeout;

            /// <summary>
            /// the name of the event on the server
            /// </summary>
            private readonly string _senderEventName;


            /// <summary>
            /// sends log threaded
            /// </summary>
            /// <param name="log"></param>
            /// <param name="serverUrl"></param>
            /// <param name="acceptSelfSigned"></param>
            /// <param name="timeout"></param>
            /// <param name="senderEventName"></param>
            public ThreadedLogSender(string log, string serverUrl, bool acceptSelfSigned = true,
                int timeout = 20000, string senderEventName = "log")
            {
                this._logSerialized = log;
                this._serverUrl = serverUrl;
                _senderEventName = senderEventName;
                _timeout = timeout;
                _acceptSelfSigned = acceptSelfSigned;
            }

            /// <summary>
            /// Sends a log
            /// </summary>
            public void Send()
            {
                new Thread(Sender).Start();
            }

            /// <summary>
            /// Sends a log
            /// </summary>
            private void Sender()
            {
                try
                {
                    SocketIoClient.RetrieveSingleValue<SimpleResult>(_serverUrl,
                  "closer", _senderEventName, _logSerialized);

                }
                catch (System.Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Sends an Log
        /// </summary>
        /// <param name="log">FnLog.LogWrap</param>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        public static void SendLog(Log log, string serverUrl, bool acceptSelfSigned = true)
        {
            var threadedLogSender = new ThreadedLogSender(log.Serialize(), serverUrl, acceptSelfSigned);
            threadedLogSender.Send();
        }

        /// <summary>
        /// sends multiple logs as serialized json list of log
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="serverUrl"></param>
        /// <param name="acceptSelfSigned"></param>
        public static void SendLogPackage(List<Log> logs, string serverUrl, bool acceptSelfSigned = true)
        {
             var logPak = JsonConvert.SerializeObject(logs, Formatting.Indented).Trim().
                 Replace(System.Environment.NewLine, "");
            var threadedLogSender = new ThreadedLogSender(logPak, serverUrl, acceptSelfSigned, senderEventName: "logPak");
            threadedLogSender.Send();
        }

        /// <summary>
        /// testing atm!
        /// retrieves logs from the fnLog server
        /// </summary>
        /// <param name="logRequest"></param>
        /// <param name="serverUrl"></param>
        /// <param name="acceptSelfSigned"></param>
        /// <returns></returns>
        public static List<Log> RetrieveLog(LogRequest logRequest, string serverUrl, bool acceptSelfSigned = true) 
        {
            return SocketIoClient.RetrieveSingleValue<List<Log>>(serverUrl,
                "logRequestResult", "logRequest", logRequest.Serialize());
        }

        
    }
}
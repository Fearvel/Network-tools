using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.FnLog;
using de.fearvel.net.DataTypes.SocketIo;
using de.fearvel.net.SocketIo;
using Quobject.SocketIoClientDotNet.Client;
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
            private readonly Log _log;

            /// <summary>
            /// Server URL
            /// </summary>
            private readonly string _serverUrl;

            /// <summary>
            /// boll for accepting self signed certificates
            /// </summary>
            private readonly bool _acceptSelfSigned;

            /// <summary>
            /// timeout in ms
            /// </summary>
            private readonly int _timeout;

            public ThreadedLogSender(Log log, string serverUrl, bool acceptSelfSigned = true,
                int timeout = 20000)
            {
                this._log = log;
                this._serverUrl = serverUrl;
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
                  "closer", "log", _log.Serialize());

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
            var threadedLogSender = new ThreadedLogSender(log, serverUrl, acceptSelfSigned);
            threadedLogSender.Send();
        }

        public static List<Log> RetrieveLog(LogRequest logRequest, string serverUrl, bool acceptSelfSigned = true) 
        {
            return SocketIoClient.RetrieveSingleValue<List<Log>>(serverUrl,
                "logRequestResult", "logRequest", logRequest.Serialize());
        }

        
    }
}
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.SocketIo;
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
            private readonly FnLog.LogWrap _log;

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

            public ThreadedLogSender(FnLog.LogWrap log, string serverUrl, bool acceptSelfSigned = true,
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
                var delay = new TimeDelay(_timeout);
                bool wait = true;
                var socket = SocketIo.SocketIoClient.GetSocket(_serverUrl, _acceptSelfSigned);

                socket.On(Socket.EVENT_CONNECT, () => { socket.Emit("log", _log.Serialize()); });

                socket.On("closingAnswer", (data) => { socket.Disconnect(); });

                socket.On("info", (data) => { });

                socket.On(Socket.EVENT_DISCONNECT, () => { wait = false; });
                while (wait && delay.Locked)
                {
                }
            }
        }

        /// <summary>
        /// Sends an Log
        /// </summary>
        /// <param name="log">FnLog.LogWrap</param>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        public static void SendLog(FnLog.LogWrap log, string serverUrl, bool acceptSelfSigned = true)
        {
            var a = new ThreadedLogSender(log, serverUrl, acceptSelfSigned);
            a.Send();
            //new ThreadedLogSender(log, serverUrl, acceptSelfSigned).Send();
        }

        /// <summary>
        /// Retrieve all Logs Async
        /// </summary>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="accessKey">accessKey</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns>Task of DataTable</returns>
        public static async Task<DataTable> RetrieveLogsAsync(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true, int timeout = 5000) =>
            await AsyncLogRetriever(serverUrl, accessKey, acceptSelfSigned, timeout);

        /// <summary>
        /// Retrieve all Logs Async
        /// </summary>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="accessKey">accessKey</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns>Task of DataTable</returns>
        private static Task<DataTable> AsyncLogRetriever(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true, int timeout = 5000) =>
            Task.Run<DataTable>(() => RetrieveLogs(serverUrl, accessKey, acceptSelfSigned, timeout));

        /// <summary>
        /// Retrieve all logs
        /// needs an accessKey
        /// </summary>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="accessKey">accessKey</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns></returns>
        public static DataTable RetrieveLogs(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true, int timeout = 5000)
        {
            var delay = new TimeDelay(timeout);
            bool wait = true;
            bool result = false;
            DataTable dt = null;

            var socket = SocketIo.SocketIoClient.GetSocket(serverUrl, acceptSelfSigned);
            socket.On(Socket.EVENT_CONNECT, () => { socket.Emit("retrieve", accessKey.Serialize()); });

            socket.On("logTable", (data) =>
            {
                dt = JsonConvert.DeserializeObject<DataTable>(data.ToString());
                result = true;
                socket.Disconnect();
            });

            socket.On("closingAnswer", (data) =>
            {
                result = SimpleResult.DeSerialize((string) data).Result;
                socket.Disconnect();
            });

            socket.On(Socket.EVENT_DISCONNECT, () => { wait = false; });

            while (wait && delay.Locked)
            {
            }

            if (!result)
                throw new AccessKeyDeclinedException("AccessKey INVALID");
            return dt;
        }
    }
}
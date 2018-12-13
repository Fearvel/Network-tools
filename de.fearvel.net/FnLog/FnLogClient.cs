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
    public class FnLogClient
    {
        private class ThreadedLogSender
        {
            private FnLog.LogWrap _log;
            private string _serverUrl;
            private bool _acceptSelfSigned;
            private int _timeout;

            public ThreadedLogSender(FnLog.LogWrap log, string serverUrl, bool acceptSelfSigned = true, int timeout = 20000)
            {
                this._log = log;
                this._serverUrl = serverUrl;
                _timeout = timeout;
                _acceptSelfSigned = acceptSelfSigned;
            }

            public void Send()
            {
                new Thread(Sender).Start(); 
            }

            private void Sender()
            {
                var delay = new TimeDelay(_timeout);
                bool wait = true;
                var socket = SocketIo.SocketIoClient.GetSocket(_serverUrl, _acceptSelfSigned);
                
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    socket.Emit("log", _log.Serialize());
                });

                socket.On("closingAnswer", (data) =>
                {
                    socket.Disconnect();
                });

                socket.On("info", (data) =>
                {
                });

                socket.On(Socket.EVENT_DISCONNECT, () =>
                {
                    wait = false;
                });
                while (wait && delay.Locked) { }
            }
        }

        public static void SendLog(FnLog.LogWrap log, string serverUrl, bool acceptSelfSigned = true)
        {
            var a = new ThreadedLogSender(log, serverUrl, acceptSelfSigned);
            a.Send();
            //new ThreadedLogSender(log, serverUrl, acceptSelfSigned).Send();
        }


        public static async Task<DataTable> RetrieveLogsAsync(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true, int timeout = 5000) => 
            await AsyncLogRetriever(serverUrl, accessKey, acceptSelfSigned, timeout);

        private static Task<DataTable> AsyncLogRetriever(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true, int timeout = 5000) =>
             Task.Run<DataTable>(() => RetrieveLogs(serverUrl, accessKey, acceptSelfSigned, timeout));


        public static DataTable RetrieveLogs(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true, int timeout = 5000)
        {
            var delay = new TimeDelay(timeout);
            bool wait = true;
            bool result = false;
            DataTable dt = null;

            var socket = SocketIo.SocketIoClient.GetSocket(serverUrl, acceptSelfSigned);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("retrieve", accessKey.Serialize());

            });

            socket.On("logTable", (data) =>
            {
                dt = JsonConvert.DeserializeObject<DataTable>(data.ToString());
                result = true;
                socket.Disconnect();
            });

            socket.On("closingAnswer", (data) =>
            {
                result = SimpleResult.DeSerialize((string)data).Result;
                socket.Disconnect();
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                wait = false;
            });

            while (wait && delay.Locked) { }
            if (!result)
                throw new AccessKeyDeclinedException("AccessKey INVALID");
            return dt;
        }
    }
}

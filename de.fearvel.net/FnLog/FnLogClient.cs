using System;
using System.Data;
using System.Diagnostics;
using de.fearvel.net.DataTypes;
using de.fearvel.net.Exceptions;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;

namespace de.fearvel.net.FnLog
{
    public class FnLogClient
    {
        public void SendLog(Log log, string serverUrl, bool acceptSelfSigned = true)
        {
            DateTime startTime = DateTime.Now;
            bool wait = true;

            var socket = GetSocket(serverUrl, acceptSelfSigned);
            
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("log", log.Serialize());
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
            while (wait && startTime.AddSeconds(20).CompareTo(DateTime.Now) >= 0) { }

        }

        public DataTable RetrieveAllLogs(string serverUrl, ValueWrap accessKey,
            bool acceptSelfSigned = true)
        {
            DateTime startTime = DateTime.Now;
            bool wait = true;
            bool result = false;
            DataTable dt = null;

            var socket = GetSocket(serverUrl, acceptSelfSigned);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("retrieve", accessKey.Serialize());
            });

            socket.On("LogTable", (data) =>
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

            while (wait && startTime.AddSeconds(20).CompareTo(DateTime.Now) >= 0) { }
            if (!result)
                throw new AccessKeyDeclinedException("AccessKey INVALID");
            return dt;
        }

        private Socket GetSocket(string serverUrl, bool acceptSelfSigned = true) =>
            acceptSelfSigned ?
            IO.Socket(serverUrl, CreateOptionsSecure()) :
            IO.Socket(serverUrl);

        public static IO.Options CreateOptionsSecure() => new IO.Options
        { Secure = true, IgnoreServerCertificateValidation = true };
    }
}

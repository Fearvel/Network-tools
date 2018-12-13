using System;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using Quobject.SocketIoClientDotNet.Client;

namespace de.fearvel.net.SocketIo
{
    public static class SocketIoClient
    {
        public static async Task<T> RetrieveSingleValueAsync<T>(string serverUrl, string receiverEventName, string senderEventName,
            string senderEventValue, bool acceptSelfSigned = true, int timeout = 5000) => 
            await AsyncSingleValueRetriever<T>(serverUrl, receiverEventName, senderEventName, senderEventValue, acceptSelfSigned, timeout);

        private static Task<T> AsyncSingleValueRetriever<T>(string serverUrl, string receiverEventName, string senderEventName,
            string senderEventValue, bool acceptSelfSigned = true, int timeout = 5000) =>
            Task.Run<T>(() => RetrieveSingleValue<T>(serverUrl, receiverEventName, senderEventName, senderEventValue, acceptSelfSigned, timeout));

        public static T RetrieveSingleValue<T>(string serverUrl, string receiverEventName, string senderEventName,
            string senderEventValue, bool acceptSelfSigned = true, int timeout = 5000)
        {
            var delay = new TimeDelay(timeout);
            bool wait = true;
            T result = default(T);
            var socket = GetSocket(serverUrl, acceptSelfSigned);
            socket.On(Socket.EVENT_CONNECT, () => { socket.Emit(senderEventName, senderEventValue); });
            socket.On(receiverEventName, (data) =>
            {
                result = data.GetType() == typeof(Newtonsoft.Json.Linq.JObject)
                    ? DataTypes.AbstractDataTypes.JsonSerializable<T>.DeSerialize(
                        ((Newtonsoft.Json.Linq.JObject) data).ToString().Replace("\r", "").Replace("\n", ""))
                    : DataTypes.AbstractDataTypes.JsonSerializable<T>.DeSerialize((string) data);
                socket.Disconnect();
            });
            socket.On(Socket.EVENT_DISCONNECT, () => { wait = false; });
            while (wait && delay.Locked) { }
            if (result.Equals(default(T)))
                throw new ResultNullOrNotReceivedException();

            return result;
        }

        public static Socket GetSocket(string serverUrl, bool acceptSelfSigned = true) =>
            acceptSelfSigned ?
                IO.Socket(serverUrl, CreateOptionsSecure()) :
                IO.Socket(serverUrl);

        public static IO.Options CreateOptionsSecure() => new IO.Options
        { Secure = true, IgnoreServerCertificateValidation = true };
    }
}

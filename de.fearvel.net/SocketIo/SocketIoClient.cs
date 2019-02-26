using System;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes;
using de.fearvel.net.DataTypes.Exceptions;
using Quobject.SocketIoClientDotNet.Client;

namespace de.fearvel.net.SocketIo
{
    /// <summary>
    /// Class for sending socketio requests 
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public static class SocketIoClient
    {
        /// <summary>
        /// Sends request and receives a serialized DataType async
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="receiverEventName">receiverEventName</param>
        /// <param name="senderEventName">senderEventName</param>
        /// <param name="senderEventValue">senderEventValue</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns></returns>
        public static async Task<T> RetrieveSingleValueAsync<T>(string serverUrl, string receiverEventName,
            string senderEventName,
            string senderEventValue, bool acceptSelfSigned = true, int timeout = 5000) =>
            await AsyncSingleValueRetriever<T>(serverUrl, receiverEventName, senderEventName, senderEventValue,
                acceptSelfSigned, timeout);

        /// <summary>
        /// Sends request and receives a serialized DataType async
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="receiverEventName">receiverEventName</param>
        /// <param name="senderEventName">senderEventName</param>
        /// <param name="senderEventValue">senderEventValue</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns></returns>
        private static Task<T> AsyncSingleValueRetriever<T>(string serverUrl, string receiverEventName,
            string senderEventName,
            string senderEventValue, bool acceptSelfSigned = true, int timeout = 5000) =>
            Task.Run<T>(() => RetrieveSingleValue<T>(serverUrl, receiverEventName, senderEventName, senderEventValue,
                acceptSelfSigned, timeout));

        /// <summary>
        /// Sends request and receives a serialized DataType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serverUrl">serverUrl</param>
        /// <param name="receiverEventName">receiverEventName</param>
        /// <param name="senderEventName">senderEventName</param>
        /// <param name="senderEventValue">senderEventValue</param>
        /// <param name="acceptSelfSigned">acceptSelfSigned</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns></returns>
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
            while (wait && delay.Locked)
            {
            }

            if (result.Equals(default(T)))
                throw new ResultNullOrNotReceivedException();
            return result;
        }

        /// <summary>
        /// Creates a Socket
        /// </summary>
        /// <param name="serverUrl">URL</param>
        /// <param name="acceptSelfSigned">boolean</param>
        /// <returns></returns>
        public static Socket GetSocket(string serverUrl, bool acceptSelfSigned = true) =>
            acceptSelfSigned ? IO.Socket(serverUrl, CreateOptionsSecure()) : IO.Socket(serverUrl);

        /// <summary>
        /// function to ignore certificate validity
        /// </summary>
        /// <returns></returns>
        public static IO.Options CreateOptionsSecure() => new IO.Options
            {Secure = true, IgnoreServerCertificateValidation = true};
    }
}
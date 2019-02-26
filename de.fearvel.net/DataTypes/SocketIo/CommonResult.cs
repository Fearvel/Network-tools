using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Result class to determine an Transaction
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class CommonResult : JsonSerializable<CommonResult>
    {
        /// <summary>
        /// Result boolean
        /// </summary>
        public bool Result;

        /// <summary>
        /// Result code
        /// </summary>
        public int Code;

        /// <summary>
        /// Message
        /// </summary>
        public string Message;

        /// <summary>
        /// bool Result contains Information about the validity of the Overall Object
        /// int Code -> errorcode
        /// string Message -> errormessage
        /// </summary>
        /// <param name="b"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public CommonResult(bool b, int code, string message)
        {
            Result = b;
            Code = code;
            Message = message;
        }
    }
}
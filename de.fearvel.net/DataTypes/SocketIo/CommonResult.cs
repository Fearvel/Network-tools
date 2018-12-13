using de.fearvel.net.DataTypes.AbstractDataTypes;
namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Result class to determine an Transaction
    /// </summary>
    public class CommonResult : JsonSerializable<CommonResult>
    {
        public bool Result;
        public int Code;
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
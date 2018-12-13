using de.fearvel.net.DataTypes.AbstractDataTypes;
namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Wrapper class for the Serializable Objects T and CommonResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OfferWrapper<T> : JsonSerializable<OfferWrapper<T>>
    {

        public T Data;
        public CommonResult Result;

        /// <summary>
        /// T data Main Object for Serialization or Deserialization
        /// CommonResult result used to determine if
        /// the data object is filled and valid
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        public OfferWrapper(T data, CommonResult result)
        {
            Data = data;
            Result = result;
        }
    }
}
using de.fearvel.net.DataTypes.AbstractDataTypes;
namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Wrapper class for the Serializable Objects T and SimpleResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleOfferWrapper<T> : JsonSerializable<SimpleOfferWrapper<T>>
    {
        public T Data;
        public SimpleResult Result;

        /// <summary>
        /// T data Main Object for Serialization or Deserialization
        /// SimpleResult used to determine if
        /// the data object is filled and valid
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        public SimpleOfferWrapper(T data, SimpleResult result)
        {
            Data = data;
            Result = result;
        }
    }
}
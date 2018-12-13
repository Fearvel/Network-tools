using de.fearvel.net.DataTypes.AbstractDataTypes;
namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Result class to determine an Transaction
    /// </summary>
    public class SimpleResult : JsonSerializable<SimpleResult>
    {
        public bool Result;

        /// <summary>
        /// bool Result contains Information about the validity of the Overall Object    
        /// </summary>
        /// <param name="b"></param>
        public SimpleResult(bool b)
        {
            Result = b;
        }
    }
}
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Result class to determine an Transaction
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class SimpleResult : JsonSerializable<SimpleResult>
    {
        /// <summary>
        /// result boolean
        /// </summary>
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
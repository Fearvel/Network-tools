using de.fearvel.net.DataTypes.AbstractDataTypes;
namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Simple Serializable Wrapper for a string Value
    /// </summary>
    public class ValueWrap : JsonSerializable<ValueWrap>
    {
        public string Val;

        /// <summary>
        /// string val a simple string
        /// </summary>
        /// <param name="val"></param>
        public ValueWrap(string val)
        {
            Val = val;
        }
    }
}
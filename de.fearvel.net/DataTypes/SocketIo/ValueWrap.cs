using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Simple Serializable Wrapper for a string Value
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ValueWrap : JsonSerializable<ValueWrap>
    {
        /// <summary>
        /// valueString
        /// </summary>
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
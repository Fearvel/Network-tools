using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Simple Key Value Wrapper
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class KeyValueWrap : JsonSerializable<KeyValueWrap>
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key;

        /// <summary>
        /// Value
        /// </summary>
        public string Val;

        /// <summary>
        /// string key -> Key
        /// string val -> Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public KeyValueWrap(string key, string val)
        {
            Key = key;
            Val = val;
        }
    }
}
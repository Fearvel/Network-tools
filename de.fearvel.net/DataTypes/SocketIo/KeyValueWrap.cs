using de.fearvel.net.DataTypes.AbstractDataTypes;
namespace de.fearvel.net.DataTypes.SocketIo
{
    /// <summary>
    /// Simple Key Value Wrapper
    /// </summary>
    public class KeyValueWrap : JsonSerializable<KeyValueWrap>
    {
        public string Key;
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
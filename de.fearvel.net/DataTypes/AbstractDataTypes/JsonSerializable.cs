using Newtonsoft.Json;
namespace de.fearvel.net.DataTypes.AbstractDataTypes
{
    /// <summary>
    /// Abstract Class for to Serialize or Deserialize to or from a JSON easier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonSerializable<T>
    {
        /// <summary>
        /// Serialize to JSON
        /// </summary>
        /// <returns></returns>
        public string Serialize() =>
            JsonConvert.SerializeObject(this, Formatting.Indented).Trim().
                Replace(System.Environment.NewLine, "");
        
        /// <summary>
        /// Deserializes from JSON
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T DeSerialize(string s) =>
            JsonConvert.DeserializeObject<T>(s);
    }
}
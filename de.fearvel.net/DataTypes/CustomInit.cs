using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// testing
    /// </summary>
   public class CustomInit : JsonSerializable<CustomInit>
   {
        /// <summary>
        /// the FnLog Server Url as string
        /// </summary>
        public string FnLogUrl;
        /// <summary>
        /// the Manastone Server Url as string
        /// </summary>
        public string ManastoneUrl;
   }
}

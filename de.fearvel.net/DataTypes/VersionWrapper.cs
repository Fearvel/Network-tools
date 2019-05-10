using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// Simple wrapper for a version(string)
    /// used for serialization
    /// </summary>
    public class VersionWrapper: JsonSerializable<VersionWrapper>
    {
        /// <summary>
        /// the Version string
        /// </summary>
        public string Version;
        }
}

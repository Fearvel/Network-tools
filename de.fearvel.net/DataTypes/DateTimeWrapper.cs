using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// Simple wrapper for datetime
    /// used for serialization
    /// </summary>
    public class DateTimeWrapper : JsonSerializable<DateTimeWrapper>
    {
        /// <summary>
        /// the DateTime variable
        /// </summary>
        public DateTime Time;
    }
}
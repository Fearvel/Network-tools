using System.Collections.Generic;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.FnLog
{
    /// <summary>
    /// A simple Wrapper class to Get a List of Logs
    /// </summary>
    public class LogSet : JsonSerializable<LogSet>
    {
        /// <summary>
        /// The List of Logs
        /// </summary>
        public List<Log> Logs;
    }
}

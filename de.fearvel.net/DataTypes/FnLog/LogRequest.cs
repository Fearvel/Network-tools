using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.FnLog
{

    /// <summary>
    /// Class for defining logRequests
    /// Log Requests are used for Sending a Token + a Filter to the Server in order
    /// to retrieve a LogSet which is basically a Select on the Log Table
    /// </summary>
    public class LogRequest : JsonSerializable<LogRequest>
    {

        /// <summary>
        /// A Security token, needed for authentication
        /// </summary>
        public string Token;

        /// <summary>
        /// A filter to define which logs should be retrieved
        /// </summary>
        public string Filter = "";

        public LogRequest(string token, string filter)
        {
            Token = token;
            Filter = filter;
        }
        public LogRequest(){}
    }
}

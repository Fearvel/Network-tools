using de.fearvel.net.DataTypes.Exceptions;
using de.fearvel.net.DataTypes.SocketIo;

namespace de.fearvel.net.Tools
{
    /// <summary>
    /// Tolls for working with data types
    /// </summary>
    public static class DataTypeTools
    {
        /// <summary>
        /// Simple check for a data type
        /// if data types wont match throws WrongDataTypeReceivedException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        public static void CheckDataType<T>(object o)
        {
            if (o.GetType() != typeof(T))
                throw new WrongDataTypeReceivedException(
                    "Got " + o.GetType() +
                    ", Expected " + typeof(T));
        }

        /// <summary>
        /// Simple check for a data type
        /// if the specified data type wont match it compares to a SimpleResult
        /// if both data types wont match throws WrongDataTypeReceivedException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        public static void CheckDataTypeIncludeSimpleResult<T>(object o)
        {
            if (o.GetType() != typeof(T) && o.GetType() != typeof(SimpleResult))
                throw new WrongDataTypeReceivedException(
                    "Got " + o.GetType() +
                    ", Expected " + typeof(T) + 
                    " or " + typeof(SimpleResult)); 
        }
    }
}
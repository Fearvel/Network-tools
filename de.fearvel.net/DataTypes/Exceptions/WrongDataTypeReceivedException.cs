using System;
namespace de.fearvel.net.DataTypes.Exceptions
{
    /// <summary>
    /// Exception for the case that a serialized object has the wrong data type
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    class WrongDataTypeReceivedException : Exception
    {
        public WrongDataTypeReceivedException() { }

        /// <summary>
        /// string s -> error message
        /// </summary>
        /// <param name="s"></param>
        public WrongDataTypeReceivedException(string s) : base(s) { }
    }
}
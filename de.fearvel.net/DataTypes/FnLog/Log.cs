using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.FnLog
{
    /// <summary>
    /// Wrapper class for FnLog
    /// </summary>
    public class Log : JsonSerializable<Log>, ICloneable
    {
        /// <summary>
        /// Id of the Log
        /// Will only be used if you retrieve data from the server
        /// Completely irrelevant for sending logs
        /// </summary>
        public long Id = 0;

        /// <summary>
        /// ProgramName
        /// </summary>
        public string ProgramName;

        /// <summary>
        /// ProgramVersion
        /// </summary>
        public string ProgramVersion;

        /// <summary>
        /// FnLogVersion
        /// </summary>
        public string FnLogClientVersion;

        /// <summary>
        /// Title
        /// </summary>
        public string Title;

        /// <summary>
        /// Description
        /// </summary>
        public string Description;

        /// <summary>
        /// LogType as int 
        /// </summary>
        public int LogType;

        /// <summary>
        /// Guid
        /// </summary>
        public string UUID;

        public Log(string programName, string programVersion, string fnLogClientVersion, string uuid, string title, string description, int logType )
        {
            ProgramName = programName;
            ProgramVersion = programVersion;
            FnLogClientVersion = fnLogClientVersion;
            UUID = uuid;
            Title = title;
            Description = description;
            LogType = logType;          
        }
        public Log() {}
        public object Clone()
        {
            return new Log(this.ProgramName,this.ProgramVersion,this.FnLogClientVersion,this.UUID, this.Title, this.Description,this.LogType);
        }
    }
}
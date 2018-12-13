using System;
using System.Collections.Generic;
using System.Net;
namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// Result Class for FnPing
    /// </summary>
    public class FnPingResult
    {
        public List<IPAddress> SuccessIpAddresses { get; private set; }
        public List<IPAddress> FailtureIpAddresses { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public TimeSpan? TimeNeeded =>  EndTime != null ? (TimeSpan?) ((DateTime) EndTime - StartTime) : null; 
        
        /// <summary>
        /// Constructor
        /// </summary>
        public FnPingResult()
        {
            SuccessIpAddresses = new List<IPAddress>();
            FailtureIpAddresses = new List<IPAddress>();
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Adds a positive Match Address
        /// </summary>
        /// <param name="ip"></param>
        public void AddSuccess(IPAddress ip) =>
            SuccessIpAddresses.Add(ip);

        /// <summary>
        /// Adds a negative Match Address
        /// </summary>
        /// <param name="ip"></param>
        public void AddFailture(IPAddress ip) =>
            FailtureIpAddresses.Add(ip);
        
        /// <summary>
        /// Sets the end time
        /// </summary>
        public void End() => EndTime = DateTime.Now;       
    }
}
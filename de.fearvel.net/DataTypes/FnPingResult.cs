using System;
using System.Collections.Generic;
using System.Net;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// Result Class for FnPing
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class FnPingResult
    {
        /// <summary>
        /// List of IPAddresses that have responded
        /// </summary>
        public List<IPAddress> SuccessIpAddresses { get; private set; }

        /// <summary>
        /// List of IPAddresses with no response
        /// </summary>
        public List<IPAddress> FailureIpAddresses { get; private set; }

        /// <summary>
        /// StartTime just for display and measurement
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// EndTime just for display and measurement
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// EndTime - StartTime
        /// </summary>
        public TimeSpan? TimeNeeded => EndTime != null ? (TimeSpan?) ((DateTime) EndTime - StartTime) : null;

        /// <summary>
        /// Constructor
        /// </summary>
        public FnPingResult()
        {
            SuccessIpAddresses = new List<IPAddress>();
            FailureIpAddresses = new List<IPAddress>();
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
            FailureIpAddresses.Add(ip);

        /// <summary>
        /// Sets the end time
        /// </summary>
        public void End() => EndTime = DateTime.Now;
    }
}
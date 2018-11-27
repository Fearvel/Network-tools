using System;
using System.Collections.Generic;
using System.Net;

namespace de.fearvel.net.DataTypes
{
    public class FnPingResult
    {
        public List<IPAddress> SuccessIpAddresses { get; private set; }
        public List<IPAddress> FailtureIpAddresses { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public TimeSpan? TimeNeeded =>  EndTime != null ? (TimeSpan?) ((DateTime) EndTime - StartTime) : null; 
        
        public FnPingResult()
        {
            SuccessIpAddresses = new List<IPAddress>();
            FailtureIpAddresses = new List<IPAddress>();
            StartTime = DateTime.Now;
        }

        public void AddSuccess(IPAddress ip) =>
            SuccessIpAddresses.Add(ip);
        
        public void AddFailture(IPAddress ip) =>
            FailtureIpAddresses.Add(ip);
        
        public void End() => EndTime = DateTime.Now;
        
    }
}

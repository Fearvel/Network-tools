using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using de.fearvel.net.DataTypes;

namespace de.fearvel.net
{
    /// <summary>
    /// Simple Ping Class
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class FnPing
    {
        /// <summary>
        /// Beginning of the ip range
        /// </summary>
        private readonly IPAddress _startIpAddress;

        /// <summary>
        /// End of the ip range
        /// </summary>
        private readonly IPAddress _endIpAddress;

        /// <summary>
        /// Constructor
        /// The Range will be defined here
        /// </summary>
        /// <param name="startIpAddress">Start of the ip Range</param>
        /// <param name="endIpAddress">End of the ip Range</param>
        public FnPing(IPAddress startIpAddress, IPAddress endIpAddress)
        {
            _startIpAddress = startIpAddress;
            _endIpAddress = endIpAddress;
        }

        /// <summary>
        /// Performs A Ping on all Addresses specified via constructor
        /// </summary>
        /// <param name="timeout">timeout in ms</param>
        /// <returns></returns>           
        public FnPingResult RangePing(int timeout = 3000) =>
            PingThreadControl(CalculateIpRangeIpAddresses(), timeout);

        /// <summary>
        /// Function to calculate an IP Address range
        /// </summary>
        /// <returns>List of IPAddress of the valid range</returns>
        private List<IPAddress> CalculateIpRangeIpAddresses()
        {
            var list = new List<IPAddress>();
            int i = _startIpAddress.GetAddressBytes()[0];
            int k = _startIpAddress.GetAddressBytes()[1];
            int j = _startIpAddress.GetAddressBytes()[2];
            int l = _startIpAddress.GetAddressBytes()[3];
            var b = _endIpAddress.GetAddressBytes()[0];
            for (; i <= _endIpAddress.GetAddressBytes()[0]; i++)
            {
                for (; k <= _endIpAddress.GetAddressBytes()[1]; k++)
                {
                    for (; j <= _endIpAddress.GetAddressBytes()[2]; j++)
                    {
                        for (; l <= _endIpAddress.GetAddressBytes()[3]; l++)
                        {
                            list.Add(
                                new IPAddress(
                                    new byte[]
                                    {
                                        (byte) i,
                                        (byte) k,
                                        (byte) j,
                                        (byte) l
                                    }
                                )
                            );
                        }

                        l = 0;
                    }

                    j = 0;
                }

                k = 0;
            }

            return list;
        }

        /// <summary>
        /// Function to handle the threaded ping
        /// </summary>
        /// <param name="ips">List of IPAddresses</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns></returns>
        private FnPingResult PingThreadControl(List<IPAddress> ips, int timeout = 5000)
        {
            var ipCount = 0;
            var ipResult = new FnPingResult();
            foreach (var ip in ips)
            {
                ipCount++;
                var loopIp = ip;

                void ThreadedPing(object state)
                {
                    if (GetIpStatus(loopIp, timeout) == IPStatus.Success)
                    {
                        ipResult.AddSuccess(loopIp);
                    }
                    else
                    {
                        ipResult.AddFailture(loopIp);
                    }

                    ipCount--;
                }

                ThreadPool.QueueUserWorkItem(ThreadedPing);
            }

            do
            {
            } while (ipCount > 0);

            ipResult.End();
            return ipResult;
        }


        /// <summary>
        /// Returns am IPStatus
        /// </summary>
        /// <param name="ip">IPAddress</param>
        /// <param name="timeout">timeout in ms</param>
        /// <returns>IPStatus</returns>
        public static IPStatus GetIpStatus(IPAddress ip, int timeout  = 5000) =>
            new System.Net.NetworkInformation.Ping().Send(ip, timeout)?.Status ?? IPStatus.BadDestination;

        public static bool PingAndCheckSuccess(IPAddress ip, int timeout = 5000) =>
            GetIpStatus(ip, timeout) == IPStatus.Success;

    }
}
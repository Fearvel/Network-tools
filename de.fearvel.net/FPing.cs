using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace de.fearvel.net
{
    public class FPing
    {
        private IPAddress StartIpAddress { get; set; }
        private IPAddress EndIpAddress { get; set; }

        public FPing(IPAddress startIpAddress, IPAddress endIpAddress)
        {
            StartIpAddress = startIpAddress;
            EndIpAddress = endIpAddress;
        }
        /// <summary>
        ///     Gets the alive network hosts.
        /// </summary>
        /// <returns></returns>
        public FPingResult RangePing(int timeout = 1)
        {
            return PingThreadControl(CalculateIpRangeIpAddresses(), timeout);
        }

        private List<IPAddress> CalculateIpRangeIpAddresses()
        {
            var list = new List<IPAddress>();
            int i = StartIpAddress.GetAddressBytes()[0];
            int k = StartIpAddress.GetAddressBytes()[1];
            int j = StartIpAddress.GetAddressBytes()[2];
            int l = StartIpAddress.GetAddressBytes()[3];
            byte b = EndIpAddress.GetAddressBytes()[0];
            for (; i <= EndIpAddress.GetAddressBytes()[0]; i++)
            {
                for (; k <= EndIpAddress.GetAddressBytes()[1]; k++)
                {
                    for (; j <= EndIpAddress.GetAddressBytes()[2]; j++)
                    {
                        for (; l <= EndIpAddress.GetAddressBytes()[3]; l++)
                        {
                            list.Add(
                                new IPAddress(
                                    new byte[]{
                                        (byte)i,
                                        (byte)k,
                                        (byte)j,
                                        (byte)l
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
        ///     Pings the specified ips.
        /// </summary>
        /// <param name="ips">The ips.</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private FPingResult PingThreadControl(List<IPAddress> ips, int timeout = 5000)
        {
            var ipCount = 0;
            var ipResult = new FPingResult();
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

        public IPStatus GetIpStatus(IPAddress ip)
        {
            return GetIpStatus(ip, 5000);
        }
        public IPStatus GetIpStatus(IPAddress ip, int timeout)
        {
            var ping = new System.Net.NetworkInformation.Ping();
            var pingReply = ping.Send(ip, timeout);
            return pingReply?.Status ?? IPStatus.BadDestination;
        }

        public TimeSpan CalculateEstimatedTime()
        {
            Double timeEst = CalculateIpRangeIpAddresses().Count * 0.025;
            return new TimeSpan((((((int)timeEst) / 60) / 60) / 24) % 60,
                ((((int)timeEst) / 60) / 60) % 60,
                (((int)timeEst) / 60) % 60,
                ((int)timeEst) % 60 + 1,
                (int)((timeEst % 1) * 1000));
        }

    }
}

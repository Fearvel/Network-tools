using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using de.fearvel.net.DataTypes;

namespace de.fearvel.net
{
    public class FnPing
    {
        private readonly IPAddress _startIpAddress;
        private readonly IPAddress _endIpAddress;

        public FnPing(IPAddress startIpAddress, IPAddress endIpAddress)
        {
            _startIpAddress = startIpAddress;
            _endIpAddress = endIpAddress;
        }

        public FnPingResult RangePing(int timeout = 1) =>
            PingThreadControl(CalculateIpRangeIpAddresses(), timeout);
        
        private List<IPAddress> CalculateIpRangeIpAddresses()
        {
            var list = new List<IPAddress>();
            int i = _startIpAddress.GetAddressBytes()[0];
            int k = _startIpAddress.GetAddressBytes()[1];
            int j = _startIpAddress.GetAddressBytes()[2];
            int l = _startIpAddress.GetAddressBytes()[3];
            byte b = _endIpAddress.GetAddressBytes()[0];
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
            do{} while (ipCount > 0);

            ipResult.End();
            return ipResult;
        }

        public IPStatus GetIpStatus(IPAddress ip) => GetIpStatus(ip, 5000);

        public IPStatus GetIpStatus(IPAddress ip, int timeout) =>
            new System.Net.NetworkInformation.Ping().Send(ip, timeout)?.Status ?? IPStatus.BadDestination;

        public TimeSpan CalculateEstimatedTime()
        {
            Double timeEst = CalculateIpRangeIpAddresses().Count * 0.035;
            return new TimeSpan((((((int)timeEst) / 60) / 60) / 24) % 60,
                ((((int)timeEst) / 60) / 60) % 60,
                (((int)timeEst) / 60) % 60,
                ((int)timeEst) % 60 + 1,
                (int)((timeEst % 1) * 1000));
        }
    }
}

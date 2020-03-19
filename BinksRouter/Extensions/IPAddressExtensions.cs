using System;
using System.Net;

namespace BinksRouter.Extensions
{
    public static class IpAddressExtensions
    {

        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress mask)
        {
            var network1 = address.ToInt() & mask.ToInt();
            var network2 = address2.ToInt() & mask.ToInt();

            return network1.Equals(network2);
        }

        public static int ToInt(this IPAddress ipAddress)
        {
            return BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);
        }
    }
}

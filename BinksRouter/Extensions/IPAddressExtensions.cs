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

        public static uint ToInt(this IPAddress ipAddress)
        {
            return ipAddress == null ? 0 : BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
        }

        public static int ToShortMask(this IPAddress ipAddress)
        {
            var inverse = ipAddress.ToInt() & uint.MaxValue;
            return (int) Math.Log(inverse, 2) + 1;
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            var ipAddressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAddressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var networkAddress = new byte[ipAddressBytes.Length];
            
            for (var i = 0; i < networkAddress.Length; i++)
            {
                networkAddress[i] = (byte)(ipAddressBytes[i] & (subnetMaskBytes[i]));
            }
            
            return new IPAddress(networkAddress);
        }
    }
}

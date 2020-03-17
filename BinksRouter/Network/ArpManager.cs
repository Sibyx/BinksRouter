using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using BinksRouter.Network.Entities;
using PacketDotNet;

namespace BinksRouter.Network
{
    public class ArpManager : ConcurrentDictionary<IPAddress, ArpRecord>
    {
        private static App CurrentApp => (App)Application.Current;

        public bool Resolve(Device sender, ArpPacket arp)
        {
            if (arp.Operation == ArpOperation.Request)
            {
                var arpResponse = new ArpPacket(
                    ArpOperation.Response,
                    arp.SenderHardwareAddress,
                    arp.SenderProtocolAddress,
                    sender.MacAddress,
                    sender.NetworkAddress
                );

                var ethernetPacket = new EthernetPacket(sender.MacAddress, arp.SenderHardwareAddress, EthernetType.None)
                    { PayloadPacket = arpResponse };

                sender.Send(ethernetPacket);
            }
            else if (arp.Operation == ArpOperation.Response)
            {
                var record = new ArpRecord(arp.SenderProtocolAddress, arp.SenderHardwareAddress);
                CurrentApp.Logging.Info($"Creating ARP record: {record}");
                return TryAdd(arp.SenderProtocolAddress, record);
            }

            return false;
        }

        public void Request(Device device, IPAddress ipAddress)
        {
            var arp = new ArpPacket(
                ArpOperation.Request,
                PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                ipAddress,
                device.MacAddress,
                device.NetworkAddress
            );

            var ethernetPacket =
                new EthernetPacket(device.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.None)
                    { PayloadPacket = arp };

            device.Send(ethernetPacket);
        }
    }
}

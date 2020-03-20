using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows;
using BinksRouter.Network.Entities;
using PacketDotNet;

namespace BinksRouter.Network
{
    public class ArpManager : ConcurrentDictionary<IPAddress, ArpRecord>
    {
        private static App CurrentApp => (App)Application.Current;

        public bool Process(Interface sender, ArpPacket arp)
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

        public void Request(Interface @interface, IPAddress ipAddress)
        {
            var arp = new ArpPacket(
                ArpOperation.Request,
                PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                ipAddress,
                @interface.MacAddress,
                @interface.NetworkAddress
            );

            var ethernetPacket =
                new EthernetPacket(@interface.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.None)
                    { PayloadPacket = arp };

            @interface.Send(ethernetPacket);
        }

        public void ClockTickEvent(object source, ElapsedEventArgs e)
        {
            // Checking expiration of ArpRecords
            foreach (var ip in Keys)
            {
                if (this[ip].TimeToDie())
                {
                    TryRemove(ip, out var record);
                    CurrentApp.Logging.Info($"Removing {record} from ARP table");
                }
            }
        }
    }
}

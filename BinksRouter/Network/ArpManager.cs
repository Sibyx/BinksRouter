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

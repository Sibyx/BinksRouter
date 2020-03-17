using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;
using BinksRouter.Annotations;
using BinksRouter.Network.Entities;
using PacketDotNet;
using SharpPcap.Npcap;

namespace BinksRouter.Network
{
    public class NetworkRouter
    {
        public List<Device> Devices { get; } = new List<Device>();
        public ArpManager ArpTable { get; } = new ArpManager();
        public RoutingTable Routes { get; } = new RoutingTable(); 
        public event EventHandler<EventArgs> RouterChange;

        private readonly Timer _clock = new Timer(Properties.Settings.Default.ClockRate);

        public NetworkRouter()
        {
            var devices = NpcapDeviceList.Instance;

            foreach (var device in devices)
            {
                if (device.Interface.FriendlyName != null)
                {
                    Devices.Add(new Device(device, PacketArrival));
                }
            }
        }

        private void PacketArrival(object sender, EthernetPacket eth)
        {
            var senderDevice = (Device)sender;

            var arp = eth.Extract<ArpPacket>();
            if (arp != null && Equals(arp.TargetProtocolAddress, senderDevice.NetworkAddress))
            {
                if (ArpTable.Resolve(senderDevice, arp))
                {
                    RouterChange?.Invoke(this, null);
                }
                return;
            }

            var ip = eth.Extract<IPPacket>();
            if (ip != null)
            {
                if (Equals(ip.DestinationAddress, senderDevice.NetworkAddress))
                {
                    Console.WriteLine("Chi chi chi, nieco pre mna!");
                }

                return;
            }
        }

        [CanBeNull]
        private PhysicalAddress arpResolve(IPAddress ipAddress)
        {
            // TODO: ProxyARP
            if (ArpTable.ContainsKey(ipAddress))
            {
                return ArpTable[ipAddress].MacAddress;
            }

            return null;
        }

        public void Start([ItemCanBeNull] IList devices)
        {
            _clock.Start();

            foreach (Device device in devices)
            {
                device?.Activate();
            }
        }

        public void Stop()
        {
            _clock.Stop();

            foreach (var device in Devices)
            {
                device.Deactivate();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows;
using BinksRouter.Annotations;
using BinksRouter.Network.Entities;
using PacketDotNet;
using SharpPcap.Npcap;

namespace BinksRouter.Network
{
    public class NetworkRouter
    {
        public List<Interface> Interfaces { get; } = new List<Interface>();
        public ArpManager ArpTable { get; } = new ArpManager();
        public RoutingTable Routes { get; } = new RoutingTable(); 
        public event EventHandler<EventArgs> RouterChange;
        private static App CurrentApp => (App)Application.Current;

        private readonly Timer _clock = new Timer(Properties.Settings.Default.ClockRate);

        public NetworkRouter()
        {
            var devices = NpcapDeviceList.Instance;

            foreach (var device in devices)
            {
                if (device.Interface.FriendlyName != null)
                {
                    var networkInterface = new Interface(device, PacketArrival);
                    networkInterface.PropertyChanged += InterfaceChanged;
                    Interfaces.Add(networkInterface);
                }
            }

            _clock.Elapsed += ClockTickEvent;
            _clock.Elapsed += ArpTable.ClockTickEvent;
            _clock.Start();
        }

        private void PacketArrival(object sender, EthernetPacket eth)
        {
            var senderDevice = (Interface)sender;

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
        private PhysicalAddress ArpResolve(IPAddress ipAddress)
        {
            // TODO: ProxyARP
            if (ArpTable.ContainsKey(ipAddress))
            {
                return ArpTable[ipAddress].MacAddress;
            }

            return null;
        }

        public void Stop()
        {
            _clock.Stop();

            foreach (var device in Interfaces)
            {
                device.IsActive = false;
            }
        }

        private void ClockTickEvent(object source, ElapsedEventArgs e)
        {
            RouterChange?.Invoke(this, null);
        }

        private void InterfaceChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is Interface myInterface)
            {
                // Routes.Remove(Routes.Single(route => route.Interface.Equals(myInterface)));
                Routes.RemoveAll(record => record.Interface != null && record.Interface.Equals(myInterface));
               
                if (myInterface.IsActive)
                {
                    Routes.Add(new Route(Route.RouteType.Connected)
                    {
                        Interface = myInterface,
                        NetworkId = myInterface.NetworkAddress,
                        NetworkMask = myInterface.NetworkMask
                    });
                    RouterChange?.Invoke(this, null);
                }
            }
        }
    }
}

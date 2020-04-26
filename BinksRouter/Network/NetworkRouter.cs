using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows;
using BinksRouter.Extensions;
using BinksRouter.Network.Entities;
using BinksRouter.Network.Protocols;
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

        private readonly Timer _clock = new Timer(1000); // 1s
        private readonly Timer _ripUpdate = new Timer(Properties.Settings.Default.RipUpdateTimer * 1000); // RipUpdateTimer is in seconds

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

            _ripUpdate.Elapsed += RipUpdateEvent;
            _ripUpdate.Start();

            Properties.Settings.Default.PropertyChanged += SettingsChanged;
        }

        private void PacketArrival(object sender, EthernetPacket eth)
        {
            var myInterface = (Interface)sender;

            var arp = eth.Extract<ArpPacket>();
            if (arp != null)
            {
                var protocol = new ArpProtocol(this, CurrentApp.Logging);
                protocol.Process(myInterface, arp);
                RouterChange?.Invoke(this, null);
                return;
            }

            if (!eth.DestinationHardwareAddress.Equals(myInterface.MacAddress) && !eth.DestinationHardwareAddress.Equals(PhysicalAddress.Parse("01-00-5E-00-00-09")))
            {
                return;
            }

            var ip = eth.Extract<IPPacket>();
            if (ip != null)
            {
                var protocol = new IpProtocol(this, CurrentApp.Logging);
                protocol.Process(myInterface, ip);
            }
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

        private void RipUpdateEvent(object source, ElapsedEventArgs e)
        {
            foreach (var @interface in Interfaces.Where(item => (item.RipEnabled)))
            {
                @interface.Send(RipPacketFactory.CreateEthernetPacket(@interface, Routes.ToPacket()));
            }
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Settings.Default.RipUpdateTimer))
            {
                _ripUpdate.Interval = Properties.Settings.Default.RipUpdateTimer * 1000;
            }
        }

        private void InterfaceChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is Interface myInterface)
            {
                try
                {

                    Routes.Remove(Routes.Single(
                        record => record.Interface != null && record.Interface.Equals(myInterface))
                    );
                }
                catch (InvalidOperationException e)
                {
                    
                }
               
                if (myInterface.IsActive)
                {
                    var route = new Route(Route.RouteType.Connected)
                    {
                        Interface = myInterface,
                        NetworkAddress = myInterface.NetworkAddress.GetNetworkAddress(myInterface.NetworkMask),
                        NetworkMask = myInterface.NetworkMask
                    };
                    
                    Routes.Add(route);

                    if (myInterface.NetworkAddress != null)
                    {
                        ArpTable[myInterface.NetworkAddress] = new ArpRecord(myInterface.NetworkAddress, myInterface.MacAddress, true);
                    }

                    RouterChange?.Invoke(this, null);
                }
            }
        }
    }
}

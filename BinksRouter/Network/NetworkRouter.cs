using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows;
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
            var myInterface = (Interface)sender;

            var arp = eth.Extract<ArpPacket>();
            if (arp != null)
            {
                var protocol = new ArpProtocol(this, CurrentApp.Logging);
                protocol.Process(myInterface, arp);
                RouterChange?.Invoke(this, null);
                return;
            }

            if (!eth.DestinationHardwareAddress.Equals(myInterface.MacAddress))
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

        private void InterfaceChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (sender is Interface myInterface)
            {
                Routes.RemoveAll(record => record.Interface != null && record.Interface.Equals(myInterface));
               
                if (myInterface.IsActive)
                {
                    var route = new Route(Route.RouteType.Connected)
                    {
                        Interface = myInterface,
                        NetworkAddress = myInterface.NetworkAddress,
                        NetworkMask = myInterface.NetworkMask
                    };
                    
                    Routes.Add(route);

                    if (route.NetworkAddress != null && route.Interface != null)
                    {
                        ArpTable[route.NetworkAddress] = new ArpRecord(route.NetworkAddress, myInterface.MacAddress, true);
                    }


                    RouterChange?.Invoke(this, null);
                }
            }
        }
    }
}

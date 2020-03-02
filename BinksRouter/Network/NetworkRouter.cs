using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public ConcurrentDictionary<string, ArpRecord> ArpRecords { get; } = new ConcurrentDictionary<string, ArpRecord>();
        public event EventHandler<EventArgs> ArpChange;

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
                if (arp.Operation == ArpOperation.Request)
                {
                    var arpResponse = new ArpPacket(
                        ArpOperation.Response,
                        arp.SenderHardwareAddress,
                        arp.SenderProtocolAddress,
                        senderDevice.MacAddress,
                        senderDevice.NetworkAddress
                    );

                    var ethernetPacket = new EthernetPacket(senderDevice.MacAddress, arp.SenderHardwareAddress, EthernetType.None)
                        { PayloadPacket = arpResponse };

                    senderDevice.Send(ethernetPacket);
                }
                else
                {
                    ArpRecords.TryAdd(arp.SenderProtocolAddress.ToString(), new ArpRecord(arp.SenderProtocolAddress, arp.SenderHardwareAddress));
                    ArpChange?.Invoke(this, null);
                }
            }
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

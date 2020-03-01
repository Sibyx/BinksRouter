using System.Collections.Generic;
using BinksRouter.Network.Entities;
using PacketDotNet;
using SharpPcap.Npcap;

namespace BinksRouter.Network
{
    public class NetworkRouter
    {
        public List<Device> Devices { get; } = new List<Device>();

        public NetworkRouter()
        {
            var devices = NpcapDeviceList.Instance;

            foreach (var device in devices)
            {
                Devices.Add(new Device(device, PacketArrival));
            }
        }

        private void PacketArrival(object sender, EthernetPacket eth)
        {
            var senderDevice = (Device)sender;
        }
    }
}

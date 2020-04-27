using BinksRouter.Extensions;
using BinksRouter.Network.Entities;
using PacketDotNet;
using SyslogLogging;

namespace BinksRouter.Network.Protocols
{
    class ArpProtocol : GenericProtocol<ArpPacket>
    {
        public ArpProtocol(NetworkRouter router, LoggingModule logger) : base(router, logger)
        {
        }

        public override void Process(Interface receiver, ArpPacket packet)
        {
            if (packet.Operation == ArpOperation.Request)
            {
                if (packet.TargetProtocolAddress.ToInt() == receiver.NetworkAddress.ToInt())
                {
                    // Odpovedz
                    var arpResponse = new ArpPacket(
                        ArpOperation.Response,
                        packet.SenderHardwareAddress,
                        packet.SenderProtocolAddress,
                        receiver.MacAddress,
                        packet.TargetProtocolAddress
                    );

                    var ethernetPacket = new EthernetPacket(receiver.MacAddress, packet.SenderHardwareAddress, EthernetType.Arp)
                        { PayloadPacket = arpResponse };

                    receiver.Send(ethernetPacket);
                }
                else if (!packet.TargetProtocolAddress.IsInSameSubnet(receiver.NetworkAddress, receiver.NetworkMask))
                {
                    // Proxy ARO
                    var route = _router.Routes.Resolve(packet.TargetProtocolAddress);

                    if (route != null)
                    {
                        var arpResponse = new ArpPacket(
                            ArpOperation.Response,
                            packet.SenderHardwareAddress,
                            packet.SenderProtocolAddress,
                            receiver.MacAddress,
                            packet.TargetProtocolAddress
                        );

                        var ethernetPacket = new EthernetPacket(receiver.MacAddress, packet.SenderHardwareAddress, EthernetType.Arp)
                            { PayloadPacket = arpResponse };

                        receiver.Send(ethernetPacket);
                    }
                }
            }
            else if (packet.Operation == ArpOperation.Response)
            {
                var record = new ArpRecord(packet.SenderProtocolAddress, packet.SenderHardwareAddress);
                _logger.Info($"Creating ARP record: {record}");
                _router.ArpTable.TryAdd(packet.SenderProtocolAddress, record);
            }
        }
    }
}

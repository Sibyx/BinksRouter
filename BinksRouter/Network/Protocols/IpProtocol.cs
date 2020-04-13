using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BinksRouter.Network.Entities;
using PacketDotNet;
using SyslogLogging;

namespace BinksRouter.Network.Protocols
{
    class IpProtocol : GenericProtocol<IPPacket>
    {
        public IpProtocol(NetworkRouter router, LoggingModule logger) : base(router, logger)
        {
        }

        public override void Process(Interface receiver, IPPacket packet)
        {
            if (Equals(packet.DestinationAddress, receiver.NetworkAddress))
            {
                _logger.Debug("Mesa called Jar Jar Binks, mesa your humble servant! (Received  IP packet)");
            }
            else if (packet.DestinationAddress.Equals(IPAddress.Parse("224.0.0.9")))
            {
                var udp = packet.Extract<UdpPacket>();
                if (udp != null && udp.DestinationPort.Equals(520))
                {
                    var protocol = new Rip2Protocol(_router, _logger);
                    protocol.Process(receiver, new RipPacket(udp.PayloadData));
                }
            }
            else
            {
                var route = _router.Routes.Resolve(packet.DestinationAddress);

                if (route != null)
                {
                    if (_router.ArpTable.TryGetValue(packet.DestinationAddress, out var record))
                    {
                        route.Interface.Send(new EthernetPacket(route.Interface.MacAddress, record.MacAddress, EthernetType.IPv4)
                        {
                            PayloadPacket = packet
                        });
                    }
                    else
                    {
                        _router.ArpTable.Request(route.Interface, packet.DestinationAddress);
                    }
                }
            }
        }
    }
}

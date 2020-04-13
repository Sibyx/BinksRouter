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
            if (packet.TargetProtocolAddress.Equals(receiver.NetworkAddress))
            {
                _router.ArpTable.Process(receiver, packet);
            }
            else if (!packet.SenderProtocolAddress.Equals(receiver.NetworkAddress))
            {
                var route = _router.Routes.Resolve(packet.TargetProtocolAddress);

                if (route != null)
                {
                    _router.ArpTable.Process(receiver, packet);
                }
                else
                {
                    _logger.Debug($"Unable to resolve {packet.TargetHardwareAddress}");
                }
            }
        }
    }
}

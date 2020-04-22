using BinksRouter.Network.Entities;
using SyslogLogging;

namespace BinksRouter.Network.Protocols
{
    class Rip2Protocol : GenericProtocol<RipPacket>
    {
        public Rip2Protocol(NetworkRouter router, LoggingModule logger) : base(router, logger)
        {
        }

        public override void Process(Interface receiver, RipPacket packet)
        {
            if (packet.Command.Equals(RipPacket.RipCommand.Response))
            {
                foreach (var ripRecord in packet.Records)
                {
                    _router.Routes.Learn(ripRecord, receiver);
                }
            }
            else if (packet.Command.Equals(RipPacket.RipCommand.Request))
            {
                
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Console.WriteLine(packet.Records.First());
            Console.WriteLine(packet.Records.Last());
        }
    }
}

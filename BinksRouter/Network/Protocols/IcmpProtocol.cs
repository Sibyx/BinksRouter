using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinksRouter.Network.Entities;
using PacketDotNet;
using SyslogLogging;

namespace BinksRouter.Network.Protocols
{
    class IcmpProtocol : GenericProtocol<IcmpV4Packet>
    {
        public IcmpProtocol(NetworkRouter router, LoggingModule logger) : base(router, logger)
        {
        }

        public override void Process(Interface receiver, IcmpV4Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}

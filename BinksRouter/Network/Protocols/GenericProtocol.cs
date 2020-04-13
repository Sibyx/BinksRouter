using BinksRouter.Network.Entities;
using SyslogLogging;

namespace BinksRouter.Network.Protocols
{
    abstract class GenericProtocol<T>
    {
        protected readonly NetworkRouter _router;
        protected readonly LoggingModule _logger;

        protected GenericProtocol(NetworkRouter router, LoggingModule logger)
        {
            _router = router;
            _logger = logger;
        }

        public abstract void Process(Interface receiver, T packet);
    }
}

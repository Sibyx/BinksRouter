using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BinksRouter.Network.Entities;
using PacketDotNet;

namespace BinksRouter.Network
{
    public class RipThread
    {
        private readonly Thread _thread;
        private readonly NetworkRouter _router;
        private bool _isRunning = false;


        // Thread methods / properties
        public void Start()
        {
            _isRunning = true;
            _thread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            Join();
        }

        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;

        public RipThread(NetworkRouter router)
        {
            _router = router;
            _thread = new Thread(_run);
        }

        private void _run()
        {
            while (_isRunning)
            {
                foreach (var @interface in _router.Interfaces.Where(item => (item.RipEnabled)))
                {
                    @interface.Send(RipPacketFactory.CreateEthernetPacket(@interface, _router.Routes.ToPacket()));
                }
                Console.WriteLine("Fuck you! With opened eyes!");
                Thread.Sleep(5000);
            }
        }
    }
}

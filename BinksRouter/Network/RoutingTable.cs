using System.Collections.Generic;
using System.Linq;
using System.Net;
using BinksRouter.Annotations;
using BinksRouter.Extensions;
using BinksRouter.Network.Entities;

namespace BinksRouter.Network
{
    public class RoutingTable : List<Route>
    {
        private readonly object _lock = new object();

        [CanBeNull]
        public Route Resolve(IPAddress ipAddress)
        {
            Route bestRoute = null;

            lock (_lock)
            {
                foreach (var route in this.Where(item => ipAddress.IsInSameSubnet(item.NetworkAddress, item.NetworkMask)))
                {
                    if (bestRoute == null || bestRoute.NetworkMask.ToInt() > route.NetworkMask.ToInt())
                    {
                        bestRoute = route;
                    }
                }
            }

            if (bestRoute == null)
            {
                return null;
            }

            return bestRoute.Interface != null ? bestRoute : Resolve(bestRoute.NextHop);
        }

        public new void Add(Route item)
        {
            lock (_lock)
            {
                if (!Contains(item))
                {
                    base.Add(item);
                }
            }
        }

        public new bool Remove(Route item)
        {
            lock (_lock)
            {
                return base.Remove(item);
            }
        }
    }
}

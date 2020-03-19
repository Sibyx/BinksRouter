using System;
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
        [CanBeNull]
        public Route Resolve(IPAddress ipAddress)
        {
            Route bestRoute = null;

            // https://docs.microsoft.com/en-us/archive/blogs/knom/ip-address-calculations-with-c-subnetmasks-networks
            // http://www.xss.wz.sk/downloads/podsiete.pdf
            // http://www.ut.fei.stuba.sk/~halas/kis/zal%202012/IP%20adresy%20PDF.pdf
            foreach (var route in this.Where(item => ipAddress.IsInSameSubnet(item.NetworkId, item.NetworkMask)))
            {
                if (bestRoute == null || bestRoute.NetworkMask.ToInt() > route.NetworkMask.ToInt())
                {
                    bestRoute = route;
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
            if (!Contains(item))
            {
                base.Add(item);
            }
        }
    }
}

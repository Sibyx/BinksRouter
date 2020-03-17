using System;
using System.Collections.Generic;
using System.Net;
using BinksRouter.Network.Entities;

namespace BinksRouter.Network
{
    public class RoutingTable : List<Route>
    {
        public Route Resolve(IPAddress ipAddress)
        {
            throw new NotImplementedException();
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

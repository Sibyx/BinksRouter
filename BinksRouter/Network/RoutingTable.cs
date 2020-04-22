using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using BinksRouter.Annotations;
using BinksRouter.Extensions;
using BinksRouter.Network.Entities;

namespace BinksRouter.Network
{
    public class RoutingTable : ObservableCollection<Route>
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

        public bool Learn(RipPacket.RipRecord ripRecord, Interface origin)
        {
            try
            {
                var existingRoute = this.Single(item => item.NetworkAddress.Equals(ripRecord.IpAddress) && item.NetworkMask.Equals(ripRecord.Mask));

                if (existingRoute.Metric > ripRecord.Metric)
                {
                    Remove(existingRoute);
                    Add(new Route(ripRecord, origin));
                }
                else
                {
                    return false;
                }
            }
            catch (InvalidOperationException e)
            {
                Add(new Route(ripRecord, origin));
            }

            return true;
        }

        public new void Add(Route item)
        {
            lock (_lock)
            {
                if (!Contains(item))
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        base.Add(item);
                    });
                }
            }
        }

        public new bool Remove(Route item)
        {
            lock (_lock)
            {
                var result = false;
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    result = base.Remove(item);
                });
                return result;
            }
        }
    }
}

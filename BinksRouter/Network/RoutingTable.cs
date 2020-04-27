using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
                    else if (bestRoute.NetworkMask.ToInt() == route.NetworkMask.ToInt() && bestRoute.Type > route.Type)
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

        public void Learn(RipPacket.RipRecord ripRecord, Interface origin)
        {
            var existingRoute = this.FirstOrDefault(item => item.NetworkAddress.Equals(ripRecord.IpAddress) && item.NetworkMask.Equals(ripRecord.Mask));


            if (existingRoute == null)
            {
                if (ripRecord.Metric < 16)
                {
                    Add(new Route(ripRecord, origin));
                }
            }
            else if (existingRoute.Status.Equals(Route.RouteStatus.Locked))
            {
                // Nenavidim cely svet, chcem sa skryt, chcem zomriet...
                // Ale hlavne nenavidim tuto podmienku, hlupy napad, co uz ked nic lepsie mi nenapada
                // bez toho aby som si rozbil Route entitu
                // Poucenie je, zivot je na picu a prinasa taketo skarede podmienky
                // Jedina utecha je, ze bolo aj horsie
                // Napriklad pocas Spanielskej inkvizicie
                // Alebo pocas 2. svetovej vojny
                // Aj napriek tomu su na Slovensku ludia, ktori si hovoria ze ten fasizmus je celkom ppci
                // Smutne je, ze s touto celou vedomostou, aj tak povazujem tuto podmienku za utrpenie
                // R'Amen!
            }
            else
            {
                if (Equals(existingRoute.Origin, origin))
                {
                    existingRoute.Status = Route.RouteStatus.Valid;
                }

                if ((Equals(existingRoute.Origin, origin) && existingRoute.Metric != ripRecord.Metric) ||
                    existingRoute.Metric > ripRecord.Metric)
                {

                    existingRoute.Status = existingRoute.Metric > ripRecord.Metric ? Route.RouteStatus.Locked : Route.RouteStatus.Valid;
                    existingRoute.NetworkAddress = ripRecord.IpAddress;
                    existingRoute.NetworkMask = ripRecord.Mask;
                    existingRoute.Metric = ripRecord.Metric;
                    existingRoute.Origin = origin;
                    existingRoute.Interface = origin;

                    if (ripRecord.NextHop.ToInt() != 0)
                    {
                        existingRoute.NextHop = ripRecord.NextHop;
                    }
                }
            }
        }

        public new void Add(Route item)
        {
            lock (_lock)
            {
                if (!Contains(item))
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        item.PropertyChanged += RouteChanged;
                        base.Add(item);
                    });
                }
            }
        }

        private void RouteChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Route route)
            {
                if (route.Status.Equals(Route.RouteStatus.Flush))
                {
                    Remove(route);
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

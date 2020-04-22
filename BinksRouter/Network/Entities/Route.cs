using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using BinksRouter.Annotations;
using BinksRouter.Extensions;

namespace BinksRouter.Network.Entities
{
    public class Route : INotifyPropertyChanged
    {
        #region Enums

        public enum RouteType
        {
            [Description("C")]
            Connected,

            [Description("S")]
            Static,

            [Description("R")]
            Rip
        }

        #endregion

        #region Private properties

        private IPAddress _networkAddress;
        private IPAddress _networkMask;
        [CanBeNull] private IPAddress _nextHop;
        [CanBeNull] private Interface _interface;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public properties

        public RouteType Type { get; }

        public IPAddress NetworkAddress
        {
            get => _networkAddress;
            set
            {
                _networkAddress = value;
                NotifyPropertyChanged(nameof(NetworkAddress));
            }
        }

        public IPAddress NetworkMask
        {
            get => _networkMask;
            set
            {
                _networkMask = value;
                NotifyPropertyChanged(nameof(NetworkMask));
            }
        }

        public IPAddress NextHop
        {
            get => _nextHop;
            set
            {
                _nextHop = value;
                NotifyPropertyChanged(nameof(NextHop));
            }
        }

        public Interface Interface
        {
            get => _interface;
            set
            {
                _interface = value;
                NotifyPropertyChanged(nameof(Interface));
            }
        }

        public readonly uint Metric;
        [CanBeNull] public readonly Interface Origin;

        #endregion

        public Route(RouteType type)
        {
            Type = type;
            Metric = 0;
            Origin = null;
        }

        public Route(RipPacket.RipRecord record, Interface origin)
        {
            Type = RouteType.Rip;
            NetworkAddress = record.IpAddress;
            NetworkMask = record.Mask;
            Metric = record.Metric;

            if (record.NextHop.ToInt() == 0)
            {
                Interface = origin;
            }
            else
            {
                NextHop = record.NextHop;
            }

            Origin = origin;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || GetType() != obj.GetType())
            {
                return false;
            }

            var route = (Route) obj;
            return GetHashCode() == route.GetHashCode();
        }

        public override int GetHashCode()
        {
            var hash = $"{NetworkAddress.ToInt()}:{NetworkMask.ToInt()}:{NextHop.ToInt()}:{Interface.Name}";
            return hash.GetHashCode();
        }
    }
}

using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using BinksRouter.Annotations;

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

        #endregion

        public Route(RouteType type)
        {
            Type = type;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

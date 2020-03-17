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
            Connected,
            Static,
            Rib
        }

        #endregion

        #region Private properties

        private RouteType _type;
        private IPAddress _networkId;
        private IPAddress _networkMask;
        [CanBeNull] private IPAddress _nextHop;
        [CanBeNull] private Device _interface;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public properties

        public RouteType Type
        {
            get => _type;
            private set
            {
                _type = value;
                NotifyPropertyChanged(nameof(Type));
            }
        }

        public IPAddress NetworkId
        {
            get => _networkId;
            set
            {
                _networkId = value;
                NotifyPropertyChanged(nameof(NetworkId));
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

        public Device Interface
        {
            get => _interface;
            set
            {
                _interface = value;
                NotifyPropertyChanged(nameof(Interface));
            }
        }

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

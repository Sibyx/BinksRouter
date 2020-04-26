using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Timers;
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

        public enum RouteStatus
        {
            [Description("Valid")]
            Valid,

            [Description("Invalid")]
            Invalid,

            [Description("Locked")]
            Locked,

            [Description("Flush")]
            Flush
        }

        #endregion

        #region Private properties

        private RouteType _type;
        private IPAddress _networkAddress;
        private IPAddress _networkMask;
        [CanBeNull] private IPAddress _nextHop;
        [CanBeNull] private Interface _interface;
        private bool _ripEnabled = false;
        private RouteStatus _status = RouteStatus.Valid;
        private uint _metric = 0;
        private readonly Timer _timer = new Timer(1000);
        private uint _timerValue = 0;
        private object _timerLock = new object();

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
                _timer.Enabled = _type.Equals(RouteType.Rip);
                NotifyPropertyChanged(nameof(Type));
            }
        }

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

        public bool RipEnabled
        {
            get => _ripEnabled;
            set
            {
                _ripEnabled = value;
                NotifyPropertyChanged(nameof(RipEnabled));
            }
        }

        public RouteStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                _timerValue = 0;
                NotifyPropertyChanged(nameof(Status));
            }
        }

        public uint Metric
        {
            get => _metric;
            private set
            {
                _metric = value;
                NotifyPropertyChanged(nameof(Metric));
            }
        }

        [CanBeNull] public readonly Interface Origin;

        public uint TimerValue
        {
            get => _timerValue;
            private set
            {
                _timerValue = value;
                NotifyPropertyChanged(nameof(TimerValue));
            }
        }

        #endregion

        public Route(RouteType type)
        {
            Type = type;
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
            _timer.Elapsed += TimerEvent;
            _timer.Start();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
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
            var hash = $"{NetworkAddress.ToInt()}:{NetworkMask.ToInt()}:{NextHop.ToInt()}:{Interface?.Name ?? ""}";
            return hash.GetHashCode();
        }

        private void TimerEvent(object source, ElapsedEventArgs e)
        {
            if (Status.Equals(RouteStatus.Valid) && _timerValue > Properties.Settings.Default.RipInvalidTimer)
            {
                Status = RouteStatus.Invalid;
            }
            else if (Status.Equals(RouteStatus.Invalid) && _timerValue > Properties.Settings.Default.RipFlushTimer)
            {
                Status = RouteStatus.Flush;
            }
            else if (Status.Equals(RouteStatus.Locked) && _timerValue > Properties.Settings.Default.RipHolddownTimer)
            {
                Status = RouteStatus.Valid;
            }

            TimerValue++;
        }
    }
}

using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using BinksRouter.Annotations;

namespace BinksRouter.Network.Entities
{
    public class ArpRecord: INotifyPropertyChanged
    {
        #region Private attributes

        private uint _timeToLive;
        private IPAddress _networkAddress;
        private PhysicalAddress _macAddress;
        private readonly object _lock = new object();
        private readonly bool _isPermanent;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public properties

        public uint TimeToLive
        {
            get => _timeToLive;
            private set
            {
                _timeToLive = value;
                OnPropertyChanged(nameof(TimeToLive));
            }
        }

        public IPAddress NetworkAddress
        {
            get => _networkAddress;
            private set
            {
                _networkAddress = value;
                OnPropertyChanged(nameof(NetworkAddress));
            }
        }

        public PhysicalAddress MacAddress
        {
            get => _macAddress;
            private set
            {
                _macAddress = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }


        #endregion

        public ArpRecord(IPAddress ip, PhysicalAddress mac, bool isPermanent = false)
        {
            NetworkAddress = ip;
            MacAddress = mac;
            _isPermanent = isPermanent;

            TimeToLive = Properties.Settings.Default.ArpRecordLifetime;
        }

        public void Refresh(IPAddress ip)
        {
            lock (_lock)
            {
                NetworkAddress = ip;
                TimeToLive = Properties.Settings.Default.ArpRecordLifetime;
            }
        }

        public bool TimeToDie()
        {
            if (_isPermanent)
            {
                return false;
            }

            lock (_lock)
            {
                return !Convert.ToBoolean(--TimeToLive);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

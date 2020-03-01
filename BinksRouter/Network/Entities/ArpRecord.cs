using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using BinksRouter.Annotations;

namespace BinksRouter.Network.Entities
{
    class ArpRecord: INotifyPropertyChanged
    {
        private uint _timeToLive;
        private IPAddress _networkAddress;
        private readonly object _arpLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;

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

        public ArpRecord(IPAddress ip)
        {
            NetworkAddress = ip;
            TimeToLive = Properties.Settings.Default.ArpRecordLifetime;
        }

        public void Refresh(IPAddress ip)
        {
            lock (_arpLock)
            {
                NetworkAddress = ip;
                TimeToLive = Properties.Settings.Default.ArpRecordLifetime;
            }
        }

        public bool TimeToDie()
        {
            lock (_arpLock)
            {
                return !Convert.ToBoolean(TimeToLive--);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

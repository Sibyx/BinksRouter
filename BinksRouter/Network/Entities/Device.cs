using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using BinksRouter.Annotations;
using SharpPcap.Npcap;

namespace BinksRouter.Network.Entities
{
    class Device : INotifyPropertyChanged
    {
        private bool _isOpened;
        private string _name;
        private IPAddress _ipAddress;
        private readonly NpcapDevice _captureDevice;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

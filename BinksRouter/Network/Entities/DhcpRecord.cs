using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using BinksRouter.Annotations;

namespace BinksRouter.Network.Entities
{
    class DhcpRecord : INotifyPropertyChanged
    {
        #region Enums

        public enum AssignmentMethod
        {
            [Description("Manual")]
            Manual,

            [Description("Automatic")]
            Automatic,

            [Description("Dynamic")]
            Dynamic
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private properties

        private PhysicalAddress _macAddress;
        private IPAddress _networkAddress;

        #endregion

        #region Public Properties

        public PhysicalAddress MacAddress
        {
            get => _macAddress;
            set
            {
                _macAddress = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }

        public IPAddress NetworkAddress
        {
            get => _networkAddress;
            set
            {
                _networkAddress = value;
                OnPropertyChanged(nameof(NetworkAddress));
            }
        }

        public readonly AssignmentMethod Type;

        #endregion

        public DhcpRecord(AssignmentMethod type)
        {
            Type = type;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

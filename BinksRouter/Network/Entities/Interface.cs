using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BinksRouter.Annotations;
using PacketDotNet;
using SharpPcap;
using SharpPcap.Npcap;

namespace BinksRouter.Network.Entities
{
    public class Interface : INotifyPropertyChanged
    {
        #region Private properties

        private bool _isActive;
        private bool _ripEnabled;
        private string _name;
        [CanBeNull] private IPAddress _networkAddress;
        [CanBeNull] private IPAddress _networkMask;
        private readonly NpcapDevice _captureDevice;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EthernetPacket> PacketReceived;

        #endregion

        #region Public properties

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (!_isActive && value)
                {
                    _captureDevice.OnPacketArrival += PacketArrival;
                    _captureDevice.Open(OpenFlags.NoCaptureLocal, 10);
                    _captureDevice.StartCapture();
                }
                else if (_isActive && !value)
                {
                    _captureDevice.StopCapture();
                    _captureDevice.Close();
                }
                else
                {
                    // Ugly as fuck, I am sorry :(
                    // Main brain hurts but I don't want to trigger NotifyPropertyChanged is is not necessary
                    return;
                }

                _isActive = value;
                NotifyPropertyChanged(nameof(IsActive));
            }
        }

        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
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

        public PhysicalAddress MacAddress => _captureDevice.MacAddress;

        public bool RipEnabled
        {
            get => _ripEnabled;
            set
            {
                _ripEnabled = value;
                NotifyPropertyChanged(nameof(RipEnabled));
            }
        }

        #endregion

        public Interface(NpcapDevice device, EventHandler<EthernetPacket> eventHandler)
        {
            PacketReceived += eventHandler;

            IsActive = false;
            Name = device.Interface.FriendlyName;
            _captureDevice = device;
        }

        public void Send(EthernetPacket ethernet)
        {
            _captureDevice.SendPacket(ethernet);
        }

        private void PacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = Packet.ParsePacket(LinkLayers.Ethernet, e.Packet.Data);

            if (packet is EthernetPacket eth)
            {
                Task.Run((() =>
                {
                    PacketReceived?.Invoke(this, eth);
                }));
            }
        }

        [NotifyPropertyChangedInvocator]
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || GetType() != obj.GetType())
            {
                return false;
            }

            var myInterface = (Interface)obj;
            return Name == myInterface.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}

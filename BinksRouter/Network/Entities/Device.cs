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
    public class Device : INotifyPropertyChanged
    {
        #region Private properties

        private bool _isActive;
        private string _name;
        [CanBeNull] private IPAddress _networkAddress;
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
            private set
            {
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

        public PhysicalAddress MacAddress => _captureDevice.MacAddress;

        #endregion

        public Device(NpcapDevice device, EventHandler<EthernetPacket> eventHandler)
        {
            PacketReceived += eventHandler;

            IsActive = false;
            Name = device.Interface.FriendlyName;
            _captureDevice = device;
        }

        public bool Activate()
        {
            if (IsActive) 
                return false;
            
            _captureDevice.OnPacketArrival += PacketArrival;
            _captureDevice.Open(OpenFlags.Promiscuous | OpenFlags.NoCaptureLocal, 10);
            _captureDevice.StartCapture();
            IsActive = true;

            return true;
        }

        public bool Deactivate()
        {
            if (IsActive)
            {
                _captureDevice.StopCapture();
                _captureDevice.Close();
                IsActive = false;
                return true;
            }

            return false;
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
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.ComponentModel;
using System.Net;
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
        private bool _isActive;
        private string _name;
        [CanBeNull] private IPAddress _networkAddress;
        private readonly NpcapDevice _captureDevice;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EthernetPacket> PacketReceived;

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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

        public Device(NpcapDevice device, EventHandler<EthernetPacket> eventHandler)
        {
            PacketReceived += eventHandler;

            IsActive = false;
            Name = device.Interface.FriendlyName;
            _captureDevice = device;
        }

        public bool Activate(IPAddress ipAddress)
        {
            if (IsActive) 
                return false;
            
            _captureDevice.OnPacketArrival += PacketArrival;
            _captureDevice.Open(OpenFlags.Promiscuous | OpenFlags.NoCaptureLocal, 10);
            _captureDevice.StartCapture();

            return true;

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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

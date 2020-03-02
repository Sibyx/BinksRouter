using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using BinksRouter.Annotations;
using BinksRouter.Network.Entities;
using PacketDotNet;

namespace BinksRouter.UI
{
    /// <summary>
    /// Interaction logic for SendArp.xaml
    /// </summary>
    public partial class SendArp : Window
    {
        [CanBeNull] private readonly IList _devices;

        public SendArp([ItemCanBeNull] IList devices)
        {
            _devices = devices;
            InitializeComponent();
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {

            if (_devices != null)
                foreach (Device device in _devices)
                {
                    var arp = new ArpPacket(
                        ArpOperation.Request,
                        PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                        IPAddress.Parse(IpAddressBox.Text),
                        device.MacAddress,
                        device.NetworkAddress
                    );

                    var ethernetPacket =
                        new EthernetPacket(device.MacAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetType.None)
                            {PayloadPacket = arp};

                    device.Send(ethernetPacket);
                }

            this.Close();
        }
    }
}

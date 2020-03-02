using System;
using System.Net;
using System.Windows;
using BinksRouter.Network.Entities;

namespace BinksRouter.UI
{
    /// <summary>
    /// Interaction logic for DeviceConfiguration.xaml
    /// </summary>
    public partial class DeviceConfiguration : Window
    {
        private readonly Device _device;

        public DeviceConfiguration(Device device)
        {
            _device = device;
            InitializeComponent();

            if (_device.NetworkAddress != null) DeviceIpAddressBox.Text = _device.NetworkAddress.ToString();
        }

        private static void InlineTry(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // ignored
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            InlineTry(() => _device.NetworkAddress = IPAddress.Parse(DeviceIpAddressBox.Text));
            this.Close();
        }
    }
}

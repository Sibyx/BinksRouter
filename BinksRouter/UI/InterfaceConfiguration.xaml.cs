using System;
using System.Net;
using System.Windows;
using BinksRouter.Network.Entities;

namespace BinksRouter.UI
{
    /// <summary>
    /// Interaction logic for InterfaceConfiguration.xaml
    /// </summary>
    public partial class InterfaceConfiguration : Window
    {
        private readonly Interface _interface;

        public InterfaceConfiguration(Interface @interface)
        {
            _interface = @interface;
            InitializeComponent();

            if (_interface.NetworkAddress != null)
            {
                DeviceIpAddressBox.Text = _interface.NetworkAddress.ToString();
            }

            if (_interface.NetworkMask != null)
            {
                DeviceMaskBox.Text = @interface.NetworkMask.ToString();
            }

            DeviceIsActiveBox.IsChecked = _interface.IsActive;
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
            InlineTry(() => _interface.NetworkAddress = IPAddress.Parse(DeviceIpAddressBox.Text));
            InlineTry(() => _interface.NetworkMask = IPAddress.Parse(DeviceMaskBox.Text));
            if (DeviceIsActiveBox?.IsChecked != null) _interface.IsActive = (bool) DeviceIsActiveBox?.IsChecked;
            Close();
        }
    }
}

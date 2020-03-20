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
                IpAddressBox.Text = _interface.NetworkAddress.ToString();
            }

            if (_interface.NetworkMask != null)
            {
                MaskBox.Text = @interface.NetworkMask.ToString();
            }

            IsActiveBox.IsChecked = _interface.IsActive;
            RipEnabledBox.IsChecked = _interface.RipEnabled;
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
            InlineTry(() => _interface.NetworkAddress = IPAddress.Parse(IpAddressBox.Text));
            InlineTry(() => _interface.NetworkMask = IPAddress.Parse(MaskBox.Text));
            if (IsActiveBox?.IsChecked != null) _interface.IsActive = (bool) IsActiveBox?.IsChecked;
            if (RipEnabledBox?.IsChecked != null) _interface.RipEnabled = (bool) RipEnabledBox.IsChecked;
            Close();
        }
    }
}

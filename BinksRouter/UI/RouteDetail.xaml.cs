using System;
using System.Linq;
using System.Net;
using System.Windows;
using BinksRouter.Network.Entities;

namespace BinksRouter.UI
{
    /// <summary>
    /// Interaction logic for RouteDetail.xaml
    /// </summary>
    public partial class RouteDetail
    {
        private readonly Route _route;
        private static App CurrentApp => (App)Application.Current;

        public RouteDetail(Route @route)
        {
            InitializeComponent();

            RouteTypeBox.Text = route.Type.ToString();
            IpAddressBox.Text = route.NetworkAddress.ToString();
            MaskBox.Text = route.NetworkMask.ToString();
            if (route.NextHop != null) NextHopBox.Text = route.NextHop.ToString();
            MetricBox.Text = route.Metric.ToString();
            if (route.Origin != null) OriginBox.Text = route.Origin.ToString();
            RipEnabledBox.IsChecked = route.RipEnabled;

            InterfaceComboBox.ItemsSource = CurrentApp.RouterInstance.Interfaces.Where(item => item.IsActive);
            InterfaceComboBox.SelectedValue = route.Interface;
            InterfaceComboBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            RemoveButton.IsEnabled = !route.Type.Equals(Route.RouteType.Connected);

            _route = route;
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
            InlineTry(() => _route.NetworkAddress = IPAddress.Parse(IpAddressBox.Text));
            InlineTry(() => _route.NetworkMask = IPAddress.Parse(MaskBox.Text));

            if (NextHopBox.Text.Trim() != "")
            {
                InlineTry(() => _route.NextHop = IPAddress.Parse(NextHopBox.Text));
            }
            else
            {
                _route.NextHop = null;
            }
            
            _route.Interface = (Interface) InterfaceComboBox.SelectedValue;
            _route.RipEnabled = RipEnabledBox.IsChecked ?? false;

            Close();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            CurrentApp.RouterInstance.Routes.Remove(_route);
            Close();
        }
    }
}

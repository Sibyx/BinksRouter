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

            // Set data sources
            InterfaceComboBox.ItemsSource = CurrentApp.RouterInstance.Interfaces.Where(item => item.IsActive);

            // Set input values
            RouteTypeBox.Text = route.Type.ToString();
            IpAddressBox.Text = route.NetworkAddress.ToString();
            MaskBox.Text = route.NetworkMask.ToString();
            MetricBox.Text = route.Metric.ToString();
            NextHopBox.Text = route.NextHop?.ToString() ?? "";
            OriginBox.Text = route.Origin?.ToString() ?? "";
            RipEnabledBox.IsChecked = route.RipEnabled;
            InterfaceComboBox.SelectedValue = route.Interface;

            // Disable readonly fields according to route type
            InterfaceComboBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            MaskBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            NextHopBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            IpAddressBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
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

            if (!CurrentApp.RouterInstance.Routes.Contains(_route))
            {
                CurrentApp.RouterInstance.Routes.Add(_route);
            }

            Close();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            CurrentApp.RouterInstance.Routes.Remove(_route);
            Close();
        }
    }
}

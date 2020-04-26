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
            DataContext = route;
            _route = route;

            // Set data sources
            InterfaceBox.ItemsSource = CurrentApp.RouterInstance.Interfaces.Where(item => item.IsActive);

            // Disable readonly fields according to route type
            InterfaceBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            NetworkMaskBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            NextHopBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            NetworkAddressBox.IsEnabled = route.Type.Equals(Route.RouteType.Static);
            RemoveButton.IsEnabled = !route.Type.Equals(Route.RouteType.Connected);
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
            InlineTry(() => _route.NetworkAddress = IPAddress.Parse(NetworkAddressBox.Text));
            InlineTry(() => _route.NetworkMask = IPAddress.Parse(NetworkMaskBox.Text));

            if (NextHopBox.Text.Trim() != "")
            {
                InlineTry(() => _route.NextHop = IPAddress.Parse(NextHopBox.Text));
            }
            else
            {
                _route.NextHop = null;
            }
            
            _route.Interface = (Interface) InterfaceBox.SelectedValue;
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

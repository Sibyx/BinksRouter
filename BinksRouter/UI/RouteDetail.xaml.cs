using System;
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

        public RouteDetail(Route @route)
        {
            InitializeComponent();

            RouteTypeBox.Text = route.Type.ToString();
            IpAddressBox.Text = route.NetworkId.ToString();
            MaskBox.Text = route.NetworkMask.ToString();
            if (route.NextHop != null) NextHopBox.Text = route.NextHop.ToString();
            if (route.Interface != null) InterfaceBox.Text = route.Interface.Name;

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
            InlineTry(() => _route.NetworkId = IPAddress.Parse(IpAddressBox.Text));
            InlineTry(() => _route.NetworkMask = IPAddress.Parse(MaskBox.Text));
            InlineTry(() => _route.NextHop = IPAddress.Parse(NextHopBox.Text));

            Close();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

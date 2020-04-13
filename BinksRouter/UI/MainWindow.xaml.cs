using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BinksRouter.Network.Entities;

namespace BinksRouter.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static App CurrentApp => (App)Application.Current;

        public MainWindow()
        {
            InitializeComponent();
            Interfaces.ItemsSource = CurrentApp.RouterInstance.Interfaces;
            ArpTable.ItemsSource = CurrentApp.RouterInstance.ArpTable.Values.ToList();
            RoutingTable.ItemsSource = CurrentApp.RouterInstance.Routes;

            CurrentApp.RouterInstance.RouterChange += RefreshArpTable;
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InterfaceDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var row = sender as DataGridRow;
                var deviceConfigurationWindow = new InterfaceConfiguration(row?.DataContext as Interface);
                deviceConfigurationWindow.ShowDialog();
            }

            RoutingTable.Items.Refresh();
        }

        private void RefreshArpTable(object sender, EventArgs eventArgs)
        {
            CurrentApp.Dispatcher?.Invoke(() =>
            {
                ArpTable.ItemsSource = CurrentApp.RouterInstance.ArpTable.Values.ToList();
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            CurrentApp.RouterInstance.Stop();
        }

        private void RouteDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var row = sender as DataGridRow;

                if (row?.DataContext is Route route)
                {
                    var routeDetailWindow = new RouteDetail(route);
                    routeDetailWindow.ShowDialog();
                    RoutingTable.Items.Refresh();
                }
            }
        }

        private void ArpRecordDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var row = sender as DataGridRow;

                if (row?.DataContext is ArpRecord record)
                {
                    CurrentApp.Logging.Info($"Manual removal of {record}");
                    CurrentApp.RouterInstance.ArpTable.TryRemove(record.NetworkAddress, out _);
                    RefreshArpTable(this, null);
                }
            }
        }

        private void ClearArpClick(object sender, RoutedEventArgs e)
        {
            CurrentApp.Logging.Warn("Manual purge of ARP table");
            CurrentApp.RouterInstance.ArpTable.Clear();
            RefreshArpTable(this, null);
        }

        private void AddRouteClick(object sender, RoutedEventArgs e)
        {
            var route = new Route(Route.RouteType.Static)
            {
                NetworkAddress = IPAddress.Parse("192.168.10.1"),
                NetworkMask = IPAddress.Parse("255.255.255.0"),
                NextHop = null
            };
            var routeDetailWindow = new RouteDetail(route);
            routeDetailWindow.ShowDialog();

            if (!CurrentApp.RouterInstance.Routes.Contains(route))
            {
                CurrentApp.RouterInstance.Routes.Add(route);
            }

            RoutingTable.Items.Refresh();
        }
    }
}

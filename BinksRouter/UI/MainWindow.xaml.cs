using System;
using System.ComponentModel;
using System.Linq;
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
            Interfaces.DataContext = CurrentApp.RouterInstance.Interfaces;
            ArpTable.DataContext = CurrentApp.RouterInstance.ArpTable.Values.ToList();
            RoutingTable.DataContext = CurrentApp.RouterInstance.Routes;

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
        }

        private void SendArpRequestClick(object sender, RoutedEventArgs e)
        {
            var sendArpRequestWindow = new SendArp(Interfaces.SelectedItems);
            sendArpRequestWindow.ShowDialog();
        }

        private void RefreshArpTable(object sender, EventArgs eventArgs)
        {
            CurrentApp.Dispatcher?.Invoke(() =>
            {
                ArpTable.DataContext = CurrentApp.RouterInstance.ArpTable.Values.ToList();
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            CurrentApp.RouterInstance.Stop();
        }

        private void RouteDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
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
    }
}

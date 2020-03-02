using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BinksRouter.Network.Entities;
using BinksRouter.UI;

namespace BinksRouter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static App CurrentApp => (App)Application.Current;

        public MainWindow()
        {
            InitializeComponent();
            DeviceTable.DataContext = CurrentApp.RouterInstance.Devices;
            ArpTable.DataContext = CurrentApp.RouterInstance.ArpRecords.Values.ToList();

            CurrentApp.RouterInstance.ArpChange += RefreshArpTable;
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeviceRecordDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                var row = sender as DataGridRow;
                var deviceConfigurationWindow = new DeviceConfiguration(row?.DataContext as Device);
                deviceConfigurationWindow.ShowDialog();
            }
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            CurrentApp.RouterInstance.Start(DeviceTable.SelectedItems);
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            CurrentApp.RouterInstance.Stop();
        }

        private void SendArpRequestClick(object sender, RoutedEventArgs e)
        {
            var sendArpRequestWindow = new SendArp(DeviceTable.SelectedItems);
            sendArpRequestWindow.ShowDialog();
        }

        private void RefreshArpTable(object sender, EventArgs eventArgs)
        {
            CurrentApp.Dispatcher?.Invoke(() =>
            {
                Console.WriteLine(CurrentApp.RouterInstance.ArpRecords.Values.ToList());
                ArpTable.DataContext = CurrentApp.RouterInstance.ArpRecords.Values.ToList();
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            CurrentApp.RouterInstance.Stop();
        }
    }
}

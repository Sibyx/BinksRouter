using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeviceRecordDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

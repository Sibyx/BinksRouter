using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BinksRouter.Annotations;

namespace BinksRouter.UI
{
    /// <summary>
    /// Interaction logic for RouterConfiguration.xaml
    /// </summary>
    public partial class RouterConfiguration : Window
    {
        public RouterConfiguration()
        {
            InitializeComponent();

            HostnameBox.Text = Properties.Settings.Default.Hostname;
            ArpRecordLifetimeBox.Text = Properties.Settings.Default.ArpRecordLifetime.ToString();
            SyslogIpBox.Text = Properties.Settings.Default.SyslogIp;
            SyslogPortBox.Text = Properties.Settings.Default.SyslogPort.ToString();
            RipUpdateTimerBox.Text = Properties.Settings.Default.RipUpdateTimer.ToString();
            RipInvalidTimerBox.Text = Properties.Settings.Default.RipInvalidTimer.ToString();
            RipFlushTimerBox.Text = Properties.Settings.Default.RipFlushTimer.ToString();
            RipHolddownTimerBox.Text = Properties.Settings.Default.RipHolddownTimer.ToString();

        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Hostname = HostnameBox.Text;
            Properties.Settings.Default.ArpRecordLifetime = Convert.ToUInt32(ArpRecordLifetimeBox.Text);
            Properties.Settings.Default.SyslogIp = SyslogIpBox.Text;
            Properties.Settings.Default.SyslogPort = Convert.ToUInt16(SyslogPortBox.Text);
            Properties.Settings.Default.RipUpdateTimer = Convert.ToUInt16(RipUpdateTimerBox.Text);
            Properties.Settings.Default.RipInvalidTimer = Convert.ToUInt16(RipInvalidTimerBox.Text);
            Properties.Settings.Default.RipFlushTimer = Convert.ToUInt16(RipFlushTimerBox.Text);
            Properties.Settings.Default.RipHolddownTimer = Convert.ToUInt16(RipHolddownTimerBox.Text);

            Close();
        }

        private void IsNumeric(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

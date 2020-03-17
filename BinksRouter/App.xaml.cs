using System.Windows;
using BinksRouter.Network;
using BinksRouter.Properties;
using SyslogLogging;

namespace BinksRouter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public NetworkRouter RouterInstance { get; } = new NetworkRouter();
        public LoggingModule Logging { get; } = new LoggingModule(Settings.Default.SyslogIp, Settings.Default.SyslogPort)
        {
            AsyncLogging = true,
            IncludeHostname = true,
            IncludeThreadId = true,
            IncludeSeverity = true,
            ConsoleEnable = false,
            IncludeUtcTimestamp = true
        };
    }
}

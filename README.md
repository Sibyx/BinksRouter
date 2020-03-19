# BinksRouter

Simple .NET based software router for Windows implemented in C# using [WPF](https://docs.microsoft.com/en-us/dotnet/framework/wpf/).

## Features

- Static routing
- [ProxyARP](https://networklessons.com/cisco/ccie-routing-switching/proxy-arp-explained) implementation
- Dynamic routing using [RIPv2](https://tools.ietf.org/html/rfc1723) protocol
- Logging to [Syslog](https://tools.ietf.org/html/rfc5424)

![](docs/screenshots/MainWindow.JPG)


## Credits

- App icon is [Cisco Router](https://icons8.com/icons/set/cisco-router) by [Icons8](https://icons8.com)
- For packet and device manipulation we use [sharppcap](https://github.com/chmorgan/sharppcap)
- Syslog logging using [SyslogLogging](https://github.com/jchristn/LoggingModule) NuGet package

---
With ❤️ and ☕️ Jakub Dubec (c) 2020

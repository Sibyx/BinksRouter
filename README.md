# BinksRouter

Simple .NET based software router for Windows implemented in C# using [WPF](https://docs.microsoft.com/en-us/dotnet/framework/wpf/).

## Features
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FSibyx%2FBinksRouter.svg?type=shield)](https://app.fossa.io/projects/git%2Bgithub.com%2FSibyx%2FBinksRouter?ref=badge_shield)


- Static routing
- [ProxyARP](https://networklessons.com/cisco/ccie-routing-switching/proxy-arp-explained) implementation
- Dynamic routing using [RIPv2](https://tools.ietf.org/html/rfc1723) protocol
- Logging to [Syslog](https://tools.ietf.org/html/rfc5424)

![](docs/screenshots/MainWindow.JPG)


## Credits

- App icon is [Cisco Router](https://icons8.com/icons/set/cisco-router) by [Icons8](https://icons8.com)
- For packet and device manipulation we use [sharppcap](https://github.com/chmorgan/sharppcap)
- Syslog logging using [SyslogLogging](https://github.com/jchristn/LoggingModule) NuGet package

## Resources

- [IP Address Calculations with C# (Subnetmasks, Networks, …)](https://docs.microsoft.com/en-us/archive/blogs/knom/ip-address-calculations-with-c-subnetmasks-networks)
- [Podsiete](http://www.xss.wz.sk/downloads/podsiete.pdf)
- [IP Adresa, maska, podsiete a priklady](http://www.ut.fei.stuba.sk/~halas/kis/zal%202012/IP%20adresy%20PDF.pdf)

---
With ❤️ and ☕️ Jakub Dubec (c) 2020


## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FSibyx%2FBinksRouter.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2FSibyx%2FBinksRouter?ref=badge_large)
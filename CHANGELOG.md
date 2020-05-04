# CHANGELOG

## 0.5.0 2020/05/04

- **Feature**: DHCP server
- **Change**: NuGet packages updated (living on a prayer)

## 0.4.0 : 2020/04/28

- **Feature**: RIPv2 🎇
- **Feature**: Router configuration window
- **Change**: ProxyARP refactor

## 0.3.0 : 2020/04/13

- **Feature**: ProxyARP
- **Change**: RoutingTable is thread-safe now
- **Feature**: Ability to remove static and RIP routes
- **Note**: Removed ARP broadcast bullshit
- **Chamge**: `Protocols` namespace introduced

## 0.2.0 : 2020/03/21

- **Feature**: Static routing baby 🍸
- **Feature**: Syslog client
- **Feature**: Interface confugration
- **Change**: ARP refactor (`ArpManager` with clock introduced)
- **Change**: Format `PhysicalAddress` with colon
- **Change**: `Device` entity renamed to `Interface`
- **Update**: Updated PacketDotNet to 1.0.4 from 1.0.3

## 0.1.0 : 2020/03/02

- **Feature**: ARPTable using `ConcurrentDictionary`
- **Feature**: Basic ARP functionality

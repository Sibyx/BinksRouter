using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using BinksRouter.Extensions;
using PacketDotNet;

namespace BinksRouter.Network.Entities
{
    public class RipPacket
    {
        #region Subclasses

        public enum RipCommand
        {
            Request = 1,
            Response = 2
        }

        public class RipRecord
        {
            public readonly IPAddress IpAddress;
            public readonly IPAddress Mask;
            public readonly IPAddress NextHop;
            public readonly uint Metric;

            public RipRecord(byte[] payload)
            {

                IpAddress = new IPAddress(payload.Skip(4).Take(4).ToArray());
                Mask = new IPAddress(payload.Skip(8).Take(4).ToArray());
                NextHop = new IPAddress(payload.Skip(12).Take(4).ToArray());
                Metric = BitConverter.ToUInt32(payload.Skip(16).Take(4).Reverse().ToArray(), 0);
            }

            public RipRecord(Route route)
            {
                IpAddress = route.NetworkAddress;
                Mask = route.NetworkMask;
                NextHop = route.NextHop;
                Metric = 1;
            }

            public override string ToString()
            {
                return $"{IpAddress}/{Mask.ToShortMask()} -> {NextHop} ({Metric})";
            }

            public byte[] ToBytes()
            {
                var stream = new MemoryStream();
                var writer = new BinaryWriter(stream);

                // Address family
                writer.Write((byte) 0x0);
                writer.Write((byte) 0x2);

                // Route Tag
                writer.Write((byte) 0x0);
                writer.Write((byte) 0x0);

                writer.Write(IpAddress.GetAddressBytes());
                writer.Write(Mask.GetAddressBytes());
                writer.Write(NextHop?.GetAddressBytes() ?? new byte[]{0x0, 0x0, 0x0, 0x0});
                writer.Write(BitConverter.GetBytes(Metric).Reverse().ToArray());

                return stream.ToArray();
            }
        }
        #endregion

        public readonly char Version;
        public readonly RipCommand Command;
        public readonly List<RipRecord> Records = new List<RipRecord>();

        public RipPacket(byte[] payload)
        {
            Command = (RipCommand)Convert.ToChar(payload[0]);
            Version = Convert.ToChar(payload[1]);

            var current = 4;
            while (current < payload.Length)
            {
                Records.Add(new RipRecord(
                    payload.Skip(current).Take(20).ToArray()
                ));
                current += 20;
            }
        }

        public RipPacket(RipCommand command)
        {
            Version = (char) 2;
            Command = command;
        }

        public byte[] ToBytes()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            writer.Write((byte) Command);
            writer.Write(Version);
            
            writer.Write((byte) 0x0);
            writer.Write((byte) 0x0);

            foreach (var record in Records)
            {
                writer.Write(record.ToBytes());
            }

            return stream.ToArray();
        }

        public static implicit operator UdpPacket(RipPacket ripPacket)
        {
            var udpPacket = new UdpPacket(42042, 520)
            {
                PayloadData = ripPacket.ToBytes()
            };

            return udpPacket;
        }
    }

    public class RipPacketFactory
    {
        public static EthernetPacket CreateEthernetPacket(Interface sourceInterface, RipPacket ripPacket)
        {
            var ipPacket = new IPv4Packet(sourceInterface.NetworkAddress, IPAddress.Parse("224.0.0.9"))
            {
                PayloadPacket = ripPacket
            };
            ipPacket.UpdateIPChecksum();

            return new EthernetPacket(sourceInterface.MacAddress, PhysicalAddress.Parse("C2-01-0C-9C-00-01"),
                EthernetType.IPv4)
            {
                PayloadPacket = ipPacket
            };
        }
    }
}

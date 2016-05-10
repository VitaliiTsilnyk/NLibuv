using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NLibuv.Native;
using Xunit;

namespace NLibuv.Tests.Native
{
	public class SockAddrTest
	{
		private static Random _Random = new Random();

		public static IEnumerable<object[]> EndPointConversionData => new[]
		{
			new object[] { "192.168.1.77", AddressFamily.InterNetwork, false, null },
			new object[] { "127.0.0.1", AddressFamily.InterNetwork, false, null },
			new object[] { "0.0.0.0", AddressFamily.InterNetwork, false, null },
			new object[] { "::1", AddressFamily.InterNetworkV6, false, null },
			new object[] { "::", AddressFamily.InterNetworkV6, false, null },
			new object[] { "2001:db8:a0b:12f0::1", AddressFamily.InterNetworkV6, false, null },
			new object[] { "fe80::", AddressFamily.InterNetworkV6, false, null },
			new object[] { "::ffff:222.1.41.90", AddressFamily.InterNetworkV6, true, "222.1.41.90" },
		};

		[Theory]
		[MemberData(nameof(EndPointConversionData))]
		public void EndPointConversionTest(string ipString, AddressFamily addressFamily, bool isIPv4MappedToIPv6, string mappedIp)
		{
			var ip = IPAddress.Parse(ipString);
			var randomPort = _Random.Next(0, 65536);
			var endPoint = new IPEndPoint(ip, randomPort);

			var resultEndPoint = SockAddr.FromIpEndPoint(endPoint).ToIpEndPoint();

			Assert.Equal(endPoint.AddressFamily, resultEndPoint.AddressFamily);
			Assert.Equal(endPoint.Address.ToString(), resultEndPoint.Address.ToString());
			Assert.Equal(randomPort, resultEndPoint.Port);

			Assert.Equal(ipString, resultEndPoint.Address.ToString());
			Assert.Equal(addressFamily, resultEndPoint.AddressFamily);
			Assert.Equal(isIPv4MappedToIPv6, resultEndPoint.Address.IsIPv4MappedToIPv6);
			if (mappedIp != null)
				Assert.Equal(mappedIp, resultEndPoint.Address.MapToIPv4().ToString());
		}

		[Fact]
		public void ArgumentNullExceptionTest()
		{
			Assert.Throws<ArgumentNullException>(() => SockAddr.FromIpEndPoint(null));
		}
	}
}
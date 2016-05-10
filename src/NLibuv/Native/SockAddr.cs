using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents native memory occupied by sockaddr structure.
	/// Windows: https://msdn.microsoft.com/en-us/library/windows/desktop/ms740496(v=vs.85).aspx
	/// Linux: https://github.com/torvalds/linux/blob/6a13feb9c82803e2b815eca72fa7a9f5561d7861/include/linux/socket.h
	/// Apple: http://www.opensource.apple.com/source/xnu/xnu-1456.1.26/bsd/sys/socket.h
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct SockAddr
	{
		private long _Field0;
		private long _Field1;
		private long _Field2;
		private long _Field3;

		/// <summary>
		/// Converts the SockAddr structure to the <see cref="IPEndPoint"/>.
		/// </summary>
		/// <returns></returns>
		public IPEndPoint ToIpEndPoint()
		{
			// Port is stored as big-endian ushort in the 3rd and 4th bytes or the structure.
			var port = ((int)(this._Field0 & 0x00FF0000) >> 8) | (int)((this._Field0 & 0xFF000000) >> 24);

			// Detect address family (IPv4 or IPv6)
			// On Mac OS the first uint8 field stores the structure length and it can not be less than 16 bytes, 
			// so the first expression will always be false. Mac OS stores the address family in the second uint8.
			if ((this._Field0 & 0x000000FF) == 2 || (this._Field0 & 0x0000FF00) == 2)
			{
				// IPv4
				return new IPEndPoint(new IPAddress((this._Field0 >> 32) & 0xFFFFFFFF), port);
			}
			else
			{
				// IPv6
				var bytes1 = BitConverter.GetBytes(this._Field1);
				var bytes2 = BitConverter.GetBytes(this._Field2);

				var bytes = new byte[16];
				for (int i = 0; i < 8; ++i)
				{
					bytes[i] = bytes1[i];
					bytes[i + 8] = bytes2[i];
				}

				return new IPEndPoint(new IPAddress(bytes), port);
			}
		}

		/// <summary>
		/// Converts the <see cref="IPEndPoint"/> to the <see cref="SockAddr"/> structure.
		/// </summary>
		/// <param name="endPoint"></param>
		/// <returns></returns>
		public static SockAddr FromIpEndPoint(IPEndPoint endPoint)
		{
			if (endPoint == null)
				throw new ArgumentNullException(nameof(endPoint));

			SockAddr addr;
			if (endPoint.AddressFamily == AddressFamily.InterNetwork)
			{
				Libuv.EnsureSuccess(Libuv.uv_ip4_addr(endPoint.Address.ToString(), endPoint.Port, out addr));
			}
			else if (endPoint.AddressFamily == AddressFamily.InterNetworkV6)
			{
				Libuv.EnsureSuccess(Libuv.uv_ip6_addr(endPoint.Address.ToString(), endPoint.Port, out addr));
			}
			else
			{
				throw new ArgumentException($"Address family '{endPoint.AddressFamily}' is not supported.", nameof(endPoint));
			}
			return addr;
		}
	}
}
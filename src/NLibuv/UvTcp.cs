using System;
using System.Net;
using System.Runtime.InteropServices;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv TCP handle (uv_tcp_t).
	/// TCP handles are used to represent both TCP streams and servers.
	/// </summary>
	public class UvTcp : UvNetworkStream
	{
		/// <summary>
		/// Initializes a new instance of <see cref="UvTcp"/> handle on the given event loop.
		/// </summary>
		/// <param name="loop"></param>
		public UvTcp(UvLoop loop) : base(loop, UvHandleType.Tcp)
		{
			Libuv.EnsureSuccess(Libuv.uv_tcp_init(loop, this));
			this.NeedsToBeClosed = true;
		}

		/// <summary>
		/// Binds the handle to the specified end point.
		/// </summary>
		/// <param name="endPoint"></param>
		public void Bind(IPEndPoint endPoint)
		{
			this.EnsureCallingThread();

			var addr = SockAddr.FromIpEndPoint(endPoint);
			Libuv.EnsureSuccess(Libuv.uv_tcp_bind(this, ref addr, 0));
		}

		/// <summary>
		/// Gets the current address to which the handle is bound.
		/// </summary>
		/// <returns></returns>
		public IPEndPoint GetPeerEndPoint()
		{
			this.EnsureCallingThread();

			SockAddr socketAddress;
			int namelen = Marshal.SizeOf<SockAddr>();
			Libuv.EnsureSuccess(Libuv.uv_tcp_getpeername(this, out socketAddress, ref namelen));

			return socketAddress.ToIpEndPoint();
		}

		/// <summary>
		/// Get the address of the peer connected to the handle.
		/// </summary>
		/// <returns></returns>
		public IPEndPoint GetSockEndPoint()
		{
			this.EnsureCallingThread();

			SockAddr socketAddress;
			int namelen = Marshal.SizeOf<SockAddr>();
			Libuv.EnsureSuccess(Libuv.uv_tcp_getsockname(this, out socketAddress, ref namelen));

			return socketAddress.ToIpEndPoint();
		}

		/// <summary>
		/// Opens an existing file descriptor or SOCKET as a TCP handle.
		/// </summary>
		/// <param name="hSocket"></param>
		public void Open(IntPtr hSocket)
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_tcp_open(this, hSocket));
		}

		/// <summary>
		/// Enables / disables Nagle’s algorithm.
		/// </summary>
		/// <param name="enable"></param>
		public void NoDelay(bool enable)
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_tcp_nodelay(this, enable ? 1 : 0));
		}

		/// <summary>
		/// Enables / disables TCP keep-alive.
		/// </summary>
		/// <param name="enable"></param>
		/// <param name="delay">Initial delay in seconds, ignored when enable is false.</param>
		public void KeepAlive(bool enable, uint delay)
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_tcp_keepalive(this, enable ? 1 : 0, delay));
		}

		/// <summary>
		/// Establishes an IPv4 or IPv6 TCP connection.
		/// </summary>
		/// <param name="endPoint">Target end point.</param>
		/// <param name="callback">Callback to be executed when the connection has been established or when a connection error happened.</param>
		/// <param name="state">State to be passed to the callback.</param>
		public void Connect(IPEndPoint endPoint, UvTcpConnectRequest.CallbackDelegate callback = null, object state = null)
		{
			this.EnsureCallingThread();

			var request = new UvTcpConnectRequest(this, endPoint, callback, state);
			request.Connect();
		}
	}
}
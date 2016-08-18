using System;
using System.Net;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a TCP connection request (uv_connect_t).
	/// </summary>
	public class UvTcpConnectRequest : UvRequest<UvTcp>
	{
		/// <summary>
		/// Callback type.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="error"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvTcpConnectRequest request, Exception error, object state);

		/// <summary>
		/// The target endpoint to connect to.
		/// </summary>
		/// <remarks>
		/// This field will be cleared before the callback invocation.
		/// </remarks>
		protected IPEndPoint EndPoint;

		/// <summary>
		/// The callback to be called after the request finish.
		/// </summary>
		/// <remarks>
		/// This field will be cleared before the callback invocation.
		/// </remarks>
		protected CallbackDelegate Callback;

		/// <summary>
		/// The state object to be passed to the callback.
		/// </summary>
		/// <remarks>
		/// This field will be cleared before the callback invocation.
		/// </remarks>
		protected object State;

		/// <summary>
		/// Initializes a new instance of the TCP connection request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="endPoint"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvTcpConnectRequest(UvTcp baseHandle, IPEndPoint endPoint, CallbackDelegate callback, object state)
			: base(baseHandle, UvRequestType.Connect)
		{
			this.EndPoint = endPoint;
			this.Callback = callback;
			this.State = state;
		}

		/// <summary>
		/// Executes the request on the base handle.
		/// </summary>
		public void Connect()
		{
			var addr = SockAddr.FromIpEndPoint(this.EndPoint);
			Libuv.EnsureSuccess(Libuv.uv_tcp_connect(this, this.BaseHandle, ref addr, _UvConnectCallback));
			this.EndPoint = null;
		}

		private static readonly UvConnectCallback _UvConnectCallback = _ConnectCallback;
		private static void _ConnectCallback(IntPtr handle, int status)
		{
			var request = FromIntPtr<UvTcpConnectRequest>(handle);

			var callback = request.Callback;
			request.Callback = null;

			var state = request.State;
			request.State = null;

			Exception error;
			Libuv.CheckStatusCode(status, out error);

			if (callback != null)
			{
				callback.Invoke(request, error, state);
			}

			request.Close();
		}
	}
}
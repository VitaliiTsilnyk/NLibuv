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
		/// <param name="status"></param>
		/// <param name="error"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvTcpConnectRequest request, int status, Exception error, object state);

		private readonly IPEndPoint _EndPoint;
		private CallbackDelegate _Callback;
		private object _State;

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
			this._EndPoint = endPoint;
			this._Callback = callback;
			this._State = state;
		}

		/// <summary>
		/// Executes the request on the base handle.
		/// </summary>
		public void Connect()
		{
			var addr = SockAddr.FromIpEndPoint(this._EndPoint);
			Libuv.EnsureSuccess(Libuv.uv_tcp_connect(this, this.BaseHandle, ref addr, _UvConnectCallback));
		}

		private static readonly UvConnectCallback _UvConnectCallback = _ConnectCallback;
		private static void _ConnectCallback(IntPtr handle, int status)
		{
			var request = FromIntPtr<UvTcpConnectRequest>(handle);

			var callback = request._Callback;
			request._Callback = null;

			var state = request._State;
			request._State = null;

			Exception error;
			Libuv.CheckStatusCode(status, out error);

			if (callback != null)
			{
				callback.Invoke(request, status, error, state);
			}

			request.Close();
		}
	}
}
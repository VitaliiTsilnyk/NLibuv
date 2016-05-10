using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a shutdown request (uv_shutdown_t).
	/// </summary>
	public class UvShutdownRequest : UvRequest<UvStream>
	{
		/// <summary>
		/// Callback type.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="status"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvShutdownRequest request, int status, object state);

		private CallbackDelegate _Callback;
		private object _State;

		/// <summary>
		/// Initializes a new instance of the shutdown request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvShutdownRequest(UvStream baseHandle, CallbackDelegate callback, object state)
			: base(baseHandle, UvRequestType.Shutdown)
		{
			this._Callback = callback;
			this._State = state;
		}

		/// <summary>
		/// Executes the request on the base handle.
		/// </summary>
		public void Shutdown()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_shutdown(this, this.BaseHandle, _UvShutdownCallback));
		}


		private static readonly UvShutdownCallback _UvShutdownCallback = _ShutdownCallback;
		private static void _ShutdownCallback(IntPtr handle, int status)
		{
			var request = FromIntPtr<UvShutdownRequest>(handle);

			var callback = request._Callback;
			request._Callback = null;

			var state = request._State;
			request._State = null;

			Exception error;
			Libuv.CheckStatusCode(status, out error);

			if (callback != null)
			{
				callback.Invoke(request, status, state);
			}

			request.Close();
		}
	}
}
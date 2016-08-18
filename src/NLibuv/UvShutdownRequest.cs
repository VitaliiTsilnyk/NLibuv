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
		/// <param name="error"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvShutdownRequest request, Exception error, object state);

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
		/// Initializes a new instance of the shutdown request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvShutdownRequest(UvStream baseHandle, CallbackDelegate callback, object state)
			: base(baseHandle, UvRequestType.Shutdown)
		{
			this.Callback = callback;
			this.State = state;
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
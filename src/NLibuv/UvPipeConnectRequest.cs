using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a pipe connect request (uv_connect_t).
	/// </summary>
	public class UvPipeConnectRequest : UvRequest<UvPipe>
	{
		/// <summary>
		/// Callback type (uv_connect_cb).
		/// </summary>
		/// <param name="request"></param>
		/// <param name="status"></param>
		/// <param name="error"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvPipeConnectRequest request, int status, Exception error, object state);

		/// <summary>
		/// The name of the pipe to connect to.
		/// </summary>
		/// <remarks>
		/// This field will be cleared before the callback invocation.
		/// </remarks>
		protected string PipeName;

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
		/// Initializes a new pipe connection request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="pipeName"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvPipeConnectRequest(UvPipe baseHandle, string pipeName, CallbackDelegate callback, object state)
			: base(baseHandle, UvRequestType.Connect)
		{
			this.PipeName = pipeName;
			this.Callback = callback;
			this.State = state;
		}

		/// <summary>
		/// Executes the request on the base handle.
		/// </summary>
		public void Connect()
		{
			Libuv.uv_pipe_connect(this, this.BaseHandle, this.PipeName, _UvConnectCallback);
			this.PipeName = null;
		}


		private static readonly UvConnectCallback _UvConnectCallback = _ConnectCallback;
		private static void _ConnectCallback(IntPtr handle, int status)
		{
			var request = FromIntPtr<UvPipeConnectRequest>(handle);

			var callback = request.Callback;
			request.Callback = null;

			var state = request.State;
			request.State = null;

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
using System;
using System.Runtime.InteropServices;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a base stream handle for all network streams.
	/// </summary>
	public abstract class UvNetworkStream : UvStream
	{
		/// <summary>
		/// Listen callback type (uv_connection_cb).
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="status"></param>
		/// <param name="error"></param>
		/// <param name="state"></param>
		public delegate void ListenCallbackDelegate(UvNetworkStream stream, int status, Exception error, object state);

		private GCHandle _ListenVitality;
		private ListenCallbackDelegate _UserListenCallback;
		private object _UserListenState;

		/// <inheritdoc/>
		protected UvNetworkStream(UvLoop loop, UvHandleType handleType)
			: base(loop, handleType)
		{
		}

		/// <inheritdoc/>
		protected override void CloseHandle()
		{
			base.CloseHandle();

			if (this._ListenVitality.IsAllocated)
				this._ListenVitality.Free();
		}

		/// <summary>
		/// This call is used in conjunction with <see cref="Listen"/> to accept incoming connections.
		/// Call this function after receiving a <see cref="ListenCallbackDelegate"/> to accept the connection.
		/// </summary>
		/// <param name="streamHandle"></param>
		public void Accept(UvStream streamHandle)
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_accept(this, streamHandle));
		}

		/// <summary>
		/// Start listening for incoming connections.
		/// </summary>
		/// <param name="backlog">Indicates the number of connections the kernel might queue.</param>
		/// <param name="callback">Callback to be called when a new incoming connection is received.</param>
		/// <param name="state">State to be passed to the callback.</param>
		public void Listen(int backlog, ListenCallbackDelegate callback, object state = null)
		{
			this.EnsureCallingThread();
			if (this._ListenVitality.IsAllocated)
			{
				throw new InvalidOperationException("Listen may not be called more than once.");
			}
			try
			{
				this._UserListenCallback = callback;
				this._UserListenState = state;
				this._ListenVitality = GCHandle.Alloc(this, GCHandleType.Normal);

				Libuv.EnsureSuccess(Libuv.uv_listen(this, backlog, _UvListenCallback));
			}
			catch
			{
				this._UserListenCallback = null;
				this._UserListenState = null;
				if (this._ListenVitality.IsAllocated)
				{
					this._ListenVitality.Free();
				}
				throw;
			}
		}


		private static readonly UvConnectionCallback _UvListenCallback = _ListenCallback;
		private static void _ListenCallback(IntPtr serverPtr, int status)
		{
			var stream = FromIntPtr<UvNetworkStream>(serverPtr);

			Exception error;
			Libuv.CheckStatusCode(status, out error);

			stream._UserListenCallback.Invoke(stream, status, error, stream._UserListenState);
		}
	}
}
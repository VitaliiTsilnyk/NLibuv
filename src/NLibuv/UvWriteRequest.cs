using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a stream write request (uv_write_t).
	/// </summary>
	public class UvWriteRequest : UvRequest<UvStream>
	{
		/// <summary>
		/// Callback type.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="error"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvWriteRequest request, Exception error, object state);

		/// <summary>
		/// The data to be sent.
		/// </summary>
		/// <remarks>
		/// This field will be cleared before the callback invocation.
		/// </remarks>
		protected IList<ArraySegment<byte>> Buffers;

		/// <summary>
		/// The callback to be called after request finish.
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

		private readonly List<GCHandle> _PinnedWriteHandles = new List<GCHandle>();

		/// <summary>
		/// Initializes a new instance of the write request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="buffers"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvWriteRequest(UvStream baseHandle, IList<ArraySegment<byte>> buffers, CallbackDelegate callback, object state) 
			: base(baseHandle, UvRequestType.Write)
		{
			this.Buffers = buffers;
			this.Callback = callback;
			this.State = state;
		}

		/// <summary>
		/// Initializes a new instance of the write request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="buffer"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvWriteRequest(UvStream baseHandle, ArraySegment<byte> buffer, CallbackDelegate callback, object state) 
			: this(baseHandle, new[] { buffer }, callback, state)
		{
		}

		/// <summary>
		/// Executes the write request on the base handle.
		/// </summary>
		public void Write()
		{
			this.EnsureCallingThread();
			try
			{
				// Build UvBuffers to write
				var buffers = this.Buffers;
				var count = buffers.Count;
				var uvBuffers = new UvBuffer[count];
				for (int i = 0; i < count; i++)
				{
					var buffer = buffers[i];
					var pinnedBufferHandle = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
					this._PinnedWriteHandles.Add(pinnedBufferHandle);

					uvBuffers[i] = Libuv.uv_buf_init(Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Array, buffer.Offset), buffer.Count);
				}

				this.Write(uvBuffers);
			}
			catch (Exception)
			{
				foreach (var gcHandle in this._PinnedWriteHandles)
				{
					gcHandle.Free();
				}
				this._PinnedWriteHandles.Clear();
				throw;
			}
		}

		internal virtual void Write(UvBuffer[] buffers)
		{
			Libuv.EnsureSuccess(Libuv.uv_write(this, this.BaseHandle, buffers, buffers.Length, _UvWriteCallback));
			this.Buffers = null;
		}

		internal static readonly UvWriteCallback _UvWriteCallback = _WriteCallback;
		internal static void _WriteCallback(IntPtr handle, int status)
		{
			var request = FromIntPtr<UvWriteRequest>(handle);

			foreach (var gcHandle in request._PinnedWriteHandles)
			{
				gcHandle.Free();
			}
			request._PinnedWriteHandles.Clear();

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
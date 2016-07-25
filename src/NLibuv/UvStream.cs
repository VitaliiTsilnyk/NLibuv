using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a base libuv stream handle (uv_stream_t).
	/// Stream handles provide an abstraction of a duplex communication channel.
	/// </summary>
	public abstract class UvStream : UvHandle
	{
		/// <summary>
		/// Allocation callback type (uv_alloc_cb).
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="suggestedSize"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public delegate ArraySegment<byte> AllocCallbackDelegate(UvStream stream, int suggestedSize, object state);

		/// <summary>
		/// Read callback type (uv_read_cb).
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="nread"></param>
		/// <param name="state"></param>
		public delegate void ReadCallbackDelegate(UvStream stream, int nread, object state);

		private GCHandle _ReadVitality;
		private AllocCallbackDelegate _UserAllocCallback;
		private ReadCallbackDelegate _UserReadCallback;
		private object _UserReadState;
		private readonly List<GCHandle> _PinnedReadHandles = new List<GCHandle>();

		/// <inheritdoc/>
		protected UvStream(UvLoop loop, UvHandleType handleType)
			: base(loop, handleType)
		{
		}

		/// <inheritdoc/>
		protected override void CloseHandle()
		{
			base.CloseHandle();

			if (this._ReadVitality.IsAllocated)
				this._ReadVitality.Free();
		}

		/// <summary>
		/// Shuts down the outgoing (write) side of a duplex stream. It waits for pending write requests to complete.
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public void Shutdown(UvShutdownRequest.CallbackDelegate callback = null, object state = null)
		{
			this.EnsureCallingThread();
			var request = new UvShutdownRequest(this, callback, state);
			request.Shutdown();
		}

		/// <summary>
		/// Starts reading data from an incoming stream.
		/// The <paramref name="readCallback"/> will be executed several times until the stream closes or <see cref="ReadStop"/> method is called.
		/// </summary>
		/// <param name="allocCallback">Allocation callback.</param>
		/// <param name="readCallback">Read callback.</param>
		/// <param name="state">State to be passed to the callbacks.</param>
		public void ReadStart(AllocCallbackDelegate allocCallback, ReadCallbackDelegate readCallback, object state = null)
		{
			this.EnsureCallingThread();
			if (this._ReadVitality.IsAllocated)
			{
				throw new InvalidOperationException("ReadStop must be called before ReadStart may be called again.");
			}

			try
			{
				this._ReadVitality = GCHandle.Alloc(this, GCHandleType.Normal);
				this._UserAllocCallback = allocCallback;
				this._UserReadCallback = readCallback;
				this._UserReadState = state;

				Libuv.EnsureSuccess(Libuv.uv_read_start(this, _UvAllocCallback, _UvReadCallback));
			}
			catch (Exception)
			{
				this._UserAllocCallback = null;
				this._UserReadCallback = null;
				this._UserReadState = null;
				if (this._ReadVitality.IsAllocated)
					this._ReadVitality.Free();
				throw;
			}
		}

		/// <summary>
		/// Stops data reading for the stream. The <see cref="ReadCallbackDelegate"/> callback will no longer be called.
		/// </summary>
		public void ReadStop()
		{
			this.EnsureCallingThread();
			if (!this._ReadVitality.IsAllocated)
			{
				throw new InvalidOperationException("ReadStart must be called before ReadStop may be called.");
			}
			this._UserAllocCallback = null;
			this._UserReadCallback = null;
			this._UserReadState = null;
			this._ReadVitality.Free();

			Libuv.EnsureSuccess(Libuv.uv_read_stop(this));
		}


		/// <summary>
		/// Writes data to the stream.
		/// </summary>
		/// <param name="buffer">Data to be written. The underlying array will be pinned until the write request will be completed.</param>
		/// <param name="callback">Callback to be called after request will be completed.</param>
		/// <param name="state">State to be passed to the callback.</param>
		public void Write(ArraySegment<byte> buffer, UvWriteRequest.CallbackDelegate callback = null, object state = null)
		{
			this.EnsureCallingThread();

			var request = new UvWriteRequest(this, callback, state);
			request.Write(buffer);
		}

		/// <summary>
		/// Writes data to the stream. Buffers are written in order.
		/// </summary>
		/// <param name="buffers">Data to be written. The underlying arrays will be pinned until the write request will be completed.</param>
		/// <param name="callback">Callback to be called after request will be completed.</param>
		/// <param name="state">State to be passed to the callback.</param>
		public void Write(IList<ArraySegment<byte>> buffers, UvWriteRequest.CallbackDelegate callback = null, object state = null)
		{
			this.EnsureCallingThread();

			var request = new UvWriteRequest(this, callback, state);
			request.Write(buffers);
		}



		private static readonly UvAllocCallback _UvAllocCallback = _AllocCallback;
		private static void _AllocCallback(IntPtr streamPtr, int suggestedSize, out UvBuffer buf)
		{
			var stream = FromIntPtr<UvStream>(streamPtr);

			try
			{
				var buffer = stream._UserAllocCallback.Invoke(stream, suggestedSize, stream._UserReadState);
				var pinnedBufferHandle = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
				stream._PinnedReadHandles.Add(pinnedBufferHandle);

				buf = Libuv.uv_buf_init(Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Array, buffer.Offset), buffer.Count);
			}
			catch (Exception)
			{
				foreach (var gcHandle in stream._PinnedReadHandles)
				{
					gcHandle.Free();
				}
				stream._PinnedReadHandles.Clear();
				buf = Libuv.uv_buf_init(IntPtr.Zero, 0);
				throw;
			}
		}

		private static readonly UvReadCallback _UvReadCallback = _ReadCallback;
		private static void _ReadCallback(IntPtr streamPtr, int nread, ref UvBuffer buf)
		{
			var stream = FromIntPtr<UvStream>(streamPtr);

			foreach (var gcHandle in stream._PinnedReadHandles)
			{
				gcHandle.Free();
			}
			stream._PinnedReadHandles.Clear();

			stream._UserReadCallback.Invoke(stream, nread, stream._UserReadState);
		}
	}
}
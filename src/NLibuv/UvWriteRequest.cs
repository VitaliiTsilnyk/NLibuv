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
		/// <param name="status"></param>
		/// <param name="state"></param>
		public delegate void CallbackDelegate(UvWriteRequest request, int status, object state);

		private CallbackDelegate _Callback;
		private object _State;

		private readonly List<GCHandle> _PinnedWriteHandles = new List<GCHandle>();

		// this message is passed to write2 because it must be non-zero-length, 
		// but it has no other functional significance
		private static readonly ArraySegment<byte> _DummyMessage = new ArraySegment<byte>(new byte[] { 1, 2, 3, 4 });

		/// <summary>
		/// Initializes a new instance of the write request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvWriteRequest(UvStream baseHandle, CallbackDelegate callback, object state) 
			: base(baseHandle, UvRequestType.Write)
		{
			this._Callback = callback;
			this._State = state;
		}

		/// <summary>
		/// Executes the write request on the base handle using data from given buffer.
		/// </summary>
		/// <param name="buffer"></param>
		public void Write(ArraySegment<byte> buffer)
		{
			this.EnsureCallingThread();
			try
			{
				var pinnedBufferHandle = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
				this._PinnedWriteHandles.Add(pinnedBufferHandle);

				var uvBuffers = new UvBuffer[1];
				uvBuffers[0] = Libuv.uv_buf_init(Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Array, buffer.Offset), buffer.Count);

				Libuv.uv_write(this, this.BaseHandle, uvBuffers, 1, _UvWriteCallback);
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

		/// <summary>
		/// Executes the write request on the base handle using data from given buffers.
		/// </summary>
		/// <param name="buffers"></param>
		public void Write(IList<ArraySegment<byte>> buffers)
		{
			this.EnsureCallingThread();
			try
			{
				var count = buffers.Count;
				var uvBuffers = new UvBuffer[count];
				for (int i = 0; i < count; i++)
				{
					var buffer = buffers[i];
					var pinnedBufferHandle = GCHandle.Alloc(buffer.Array, GCHandleType.Pinned);
					this._PinnedWriteHandles.Add(pinnedBufferHandle);

					uvBuffers[i] = Libuv.uv_buf_init(Marshal.UnsafeAddrOfPinnedArrayElement(buffer.Array, buffer.Offset), buffer.Count);
				}

				Libuv.uv_write(this, this.BaseHandle, uvBuffers, count, _UvWriteCallback);
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

		/// <summary>
		/// Executes the write request on the base handle to send given handle over a pipe.
		/// </summary>
		/// <param name="sendHandle"></param>
		public void WriteHandle(UvStream sendHandle)
		{
			this.EnsureCallingThread();

			var pipeBaseHandle = this.BaseHandle as UvPipe;
			if (pipeBaseHandle == null || !pipeBaseHandle.Ipc)
			{
				throw new InvalidOperationException("Sending handles is allowed only for pipe handles with enabled IPC.");
			}

			try
			{
				var pinnedBufferHandle = GCHandle.Alloc(_DummyMessage.Array, GCHandleType.Pinned);
				this._PinnedWriteHandles.Add(pinnedBufferHandle);

				var uvBuffers = new UvBuffer[1];
				uvBuffers[0] = Libuv.uv_buf_init(Marshal.UnsafeAddrOfPinnedArrayElement(_DummyMessage.Array, _DummyMessage.Offset), _DummyMessage.Count);

				Libuv.uv_write2(this, this.BaseHandle, uvBuffers, 1, sendHandle, _UvWriteCallback);
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

		private static readonly UvWriteCallback _UvWriteCallback = _WriteCallback;
		private static void _WriteCallback(IntPtr handle, int status)
		{
			var request = FromIntPtr<UvWriteRequest>(handle);

			foreach (var gcHandle in request._PinnedWriteHandles)
			{
				gcHandle.Free();
			}
			request._PinnedWriteHandles.Clear();

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
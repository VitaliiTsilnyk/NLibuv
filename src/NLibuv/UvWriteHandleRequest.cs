using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a stream write request that can write a handle over an IPC pipe.
	/// </summary>
    public class UvWriteHandleRequest : UvWriteRequest
	{
		/// <summary>
		/// The message to be passed to uv_write2 because it must be non-zero-length,
		/// but it has no other functional significance.
		/// </summary>
		private static readonly ArraySegment<byte>[] _DummyMessage = new[] { new ArraySegment<byte>(new byte[] { 1, 2, 3, 4 }) };

		/// <summary>
		/// The handle to be sent.
		/// </summary>
		/// <remarks>
		/// This field will be cleared before the callback invocation.
		/// </remarks>
		protected UvStream HandleToSend;

		/// <summary>
		/// Initializes a new instance of the hadnle write request.
		/// </summary>
		/// <param name="baseHandle"></param>
		/// <param name="handleToSend"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public UvWriteHandleRequest(UvPipe baseHandle, UvStream handleToSend, CallbackDelegate callback, object state)
			: base(baseHandle, _DummyMessage, callback, state)
		{
			if (!baseHandle.Ipc)
			{
				throw new ArgumentException("Sending handles is allowed only for pipe handles with enabled IPC.", nameof(handleToSend));
			}
			this.HandleToSend = handleToSend;
		}

		internal override void Write(UvBuffer[] buffers)
		{
			Libuv.uv_write2(this, this.BaseHandle, buffers, buffers.Length, this.HandleToSend, _UvWriteCallback);
			this.Buffers = null;
			this.HandleToSend = null;
		}
	}
}

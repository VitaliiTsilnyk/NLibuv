using System;
using NLibuv.Native;
using System.Runtime.InteropServices;

namespace NLibuv
{
	/// <summary>
	/// Represents a base libuv handle type (uv_handle_t).
	/// </summary>
	public abstract class UvHandle : UvResourceSafeHandle, IUvHandle
	{
		private GCHandle _ClosingHandle;

		/// <inheritdoc/>
		public UvHandleType HandleType { get; }

		/// <inheritdoc/>
		public UvLoop Loop { get; }

		/// <summary>
		/// Initializes a new instance of libuv handle object.
		/// </summary>
		/// <param name="loop">Loop, on which this handle will be initialized.</param>
		/// <param name="handleType">Type of the handle.</param>
		protected UvHandle(UvLoop loop, UvHandleType handleType)
			: base(_CalculateSize(handleType), _GetLoopThreadId(loop))
		{
			this.HandleType = handleType;
			this.Loop = loop;
		}

		/// <inheritdoc/>
		protected override void CloseHandle()
		{
			this.EnsureCallingThread();

			// Protect the managed object from being collected by GC during closing inside the libuv logic
			if (!this._ClosingHandle.IsAllocated)
			{
				this._ClosingHandle = GCHandle.Alloc(this, GCHandleType.Normal);
			}

			// Object disposing will be performed inside a callback
			Libuv.uv_close(this, _UvCloseCallback);
		}

		private static readonly UvCloseCallback _UvCloseCallback = _CloseCallback;
		private static void _CloseCallback(IntPtr handlePtr)
		{
			var handle = FromIntPtr<UvHandle>(handlePtr);

			var gcHandle = handle._ClosingHandle;
			if (gcHandle.IsAllocated)
			{
				gcHandle.Free();
			}

			// Handle was closed, it's safe to dispose it now.
			handle.Dispose();
		}


		private static int _CalculateSize(UvHandleType handleType)
		{
			return Libuv.uv_handle_size(handleType).ToInt32();
		}

		private static int _GetLoopThreadId(UvLoop loop)
		{
			if (loop == null)
				throw new ArgumentNullException(nameof(loop));

			return loop.ThreadId;
		}
	}
}
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NLibuv
{
	/// <summary>
	/// Represents a control object for operating unmanaged libuv resource.
	/// </summary>
	public abstract class UvResourceSafeHandle : SafeHandle
	{
		private readonly GCHandle _GcHandle;


		/// <inheritdoc/>
		public override bool IsInvalid => this.handle == IntPtr.Zero;

		/// <summary>
		/// Gets the ID of the thread on which the handle object was initialized.
		/// </summary>
		public int ThreadId { get; }

		/// <summary>
		/// Indicates whenever current handle was properly closed or not.
		/// </summary>
		public new bool IsClosed { get; private set; }

		/// <summary>
		/// Indicates whenever this resource handle needs to be explicitly closed before destroying.
		/// Set this value to True after the 'uv_*_init' call.
		/// </summary>
		protected bool NeedsToBeClosed { get; set; }
		
		/// <summary>
		/// Creates a new instance of libuv resource handle.
		/// </summary>
		/// <param name="size">Amount of memory to allocate required for this unmanaged resource.</param>
		/// <param name="threadId">ID of the thread on which the resource is initialized.</param>
		protected unsafe UvResourceSafeHandle(int size, int threadId)
			: base(IntPtr.Zero, true)
		{
			this.ThreadId = threadId;

			// Allocate unmanaged memory for the handle
			var ptr = this.AllocateMemory(size);
			this.SetHandle(ptr);

			// Create a weak GC handle to the current object to be able to store it inside the unmanaged handle memory
			// and be able to cast a libuv handle back to the managed handle object.
			this._GcHandle = GCHandle.Alloc(this, GCHandleType.Weak);

			// Store the GC handle to the current object in the first bytes of the unmanaged handle memory.
			// This trick uses uv_handle_t's user-defined data field.
			// http://docs.libuv.org/en/v1.x/handle.html#c.uv_handle_t.data
			*(IntPtr*)ptr = GCHandle.ToIntPtr(this._GcHandle);
		}

		/// <inheritdoc/>
		protected sealed override bool ReleaseHandle()
		{
			var ptr = this.handle;
			if (ptr != IntPtr.Zero)
			{
				if (this.NeedsToBeClosed && !this.IsClosed)
				{
					throw new UvException($"UvHandle '{this.GetType().FullName}' wasn't properly closed before disposing. Please ensure that you called the Close method.");
				}

				this.SetHandleAsInvalid();

				var gcHandle = this._GcHandle;
				if (gcHandle.IsAllocated)
				{
					gcHandle.Free();
				}
				this.FreeMemory(ptr);

			}
			return true;
		}

		/// <summary>
		/// Properly closes a handle or request and ensures that current object will be disposed correctly.
		/// </summary>
#if DOTNET_CORE
		public void Close()
#else
		public new void Close()
#endif
		{
			this.EnsureCallingThread();

			if (this.IsClosed)
				return;
			this.IsClosed = true;
			
			this.CloseHandle();
		}


		/// <summary>
		/// When overridden in a derived class, must properly close a handle or request 
		/// and ensure that current object will be disposed correctly.
		/// </summary>
		protected abstract void CloseHandle();

		/// <summary>
		/// Ensures that the code executes on the thread, on which it was initialized. Throws an exception otherwise.
		/// </summary>
		protected void EnsureCallingThread()
		{
			if (Thread.CurrentThread.ManagedThreadId != this.ThreadId)
			{
				throw new UvException("Incorrect caller thread. Loop-related handles must be called only from the loop thread.");
			}
		}

		/// <summary>
		/// Gets a handle object by handle pointer.
		/// </summary>
		/// <typeparam name="THandle"></typeparam>
		/// <param name="handle"></param>
		/// <returns></returns>
		internal static unsafe THandle FromIntPtr<THandle>(IntPtr handle)
		{
			// Get GC handle from a pointer to it, stored in the first bytes of the unmanaged handle memory.
			var gcHandle = GCHandle.FromIntPtr(*(IntPtr*)handle);

			return (THandle)gcHandle.Target;
		}

		/// <summary>
		/// Allocates an unmanaged memory.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		protected virtual IntPtr AllocateMemory(int size)
		{
			return Marshal.AllocHGlobal(size);
		}

		/// <summary>
		/// Frees allocated unmanaged memory.
		/// </summary>
		/// <param name="ptr"></param>
		protected virtual void FreeMemory(IntPtr ptr)
		{
			Marshal.FreeHGlobal(ptr);
		}
	}
}
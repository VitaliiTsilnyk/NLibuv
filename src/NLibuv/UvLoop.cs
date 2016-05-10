using System;
using System.Threading;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv event loop (uv_loop_t).
	/// The event loop is the central part of libuv’s functionality.
	/// It takes care of polling for i/o and scheduling callbacks to be run based on different sources of events.
	/// </summary>
	public class UvLoop : UvResourceSafeHandle
	{
		/// <summary>
		/// Initializes a new instance of <see cref="UvLoop"/> on current thread.
		/// All calls to this loop and it's handles must be invoked from the same thread.
		/// </summary>
		public UvLoop()
			: base(_CalculateSize(), Thread.CurrentThread.ManagedThreadId)
		{
			Libuv.EnsureSuccess(Libuv.uv_loop_init(this));
			this.NeedsToBeClosed = true;
		}

		/// <inheritdoc/>
		protected override void CloseHandle()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_loop_close(this));
			this.Dispose();
		}

		/// <summary>
		/// Runs the event loop in specified mode.
		/// </summary>
		/// <param name="mode">Loop run mode.</param>
		/// <returns>Run status (depends on specified mode).</returns>
		public int Run(UvLoopRunMode mode = UvLoopRunMode.RunDefault)
		{
			this.EnsureCallingThread();
			return Libuv.EnsureSuccess(Libuv.uv_run(this, mode));
		}

		/// <summary>
		/// Stops the event loop, causing <see cref="Run"/> method to end as soon as possible.
		/// </summary>
		public void Stop()
		{
			this.EnsureCallingThread();
			Libuv.uv_stop(this);
		}


		private static int _CalculateSize()
		{
			return Libuv.uv_loop_size().ToInt32();
		}
	}
}
using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv async handle (uv_async_t).
	/// Async handles allow the user to “wakeup” the event loop and get a callback called from another thread.
	/// </summary>
	public class UvAsync : UvHandle
	{
		private readonly Action<UvAsync> _CallbackAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="UvAsync"/> handle.
		/// </summary>
		/// <param name="loop">Loop, on which this handle will be initialized.</param>
		/// <param name="callbackAction">A callback, which will be executed after a loop will receive an async signal from this handle.</param>
		public UvAsync(UvLoop loop, Action<UvAsync> callbackAction)
			: base(loop, UvHandleType.Async)
		{
			if (callbackAction == null)
				throw new ArgumentNullException(nameof(callbackAction));

			this._CallbackAction = callbackAction;
			Libuv.EnsureSuccess(Libuv.uv_async_init(loop, this, _UvAsyncCallback));
			this.NeedsToBeClosed = true;
		}

		/// <summary>
		/// Wakeup the event loop and call the async handle’s callback.
		/// It’s safe to call this function from any thread. The callback will be called on the loop thread.
		/// </summary>
		public void Send()
		{
			//this.EnsureCallingThread();  this method is thread safe
			Libuv.EnsureSuccess(Libuv.uv_async_send(this));
		}


		private static readonly UvAsyncCallback _UvAsyncCallback = _AsyncCallback;
		private static void _AsyncCallback(IntPtr handle)
		{
			var async = FromIntPtr<UvAsync>(handle);
			async._CallbackAction.Invoke(async);
		}
	}
}
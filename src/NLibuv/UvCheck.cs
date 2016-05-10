using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv check handle (uv_check_t).
	/// Check handles will run the given callback once per loop iteration, right after polling for i/o.
	/// </summary>
	public class UvCheck : UvHandle
	{
		private readonly Action<UvCheck> _CallbackAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="UvCheck"/> handle.
		/// </summary>
		/// <param name="loop">Loop, on which this handle will be initialized.</param>
		/// <param name="callbackAction">Callback, which will be invoked every loop iteration.</param>
		public UvCheck(UvLoop loop, Action<UvCheck> callbackAction)
			: base(loop, UvHandleType.Check)
		{
			if (callbackAction == null)
				throw new ArgumentNullException(nameof(callbackAction));

			this._CallbackAction = callbackAction;
			Libuv.EnsureSuccess(Libuv.uv_check_init(loop, this));
			this.NeedsToBeClosed = true;
		}

		/// <summary>
		/// Starts the handle with the callback specified in the constructor.
		/// </summary>
		public void Start()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_check_start(this, _UvCheckCallback));
		}

		/// <summary>
		/// Stop the handle, the callback will no longer be called.
		/// </summary>
		public void Stop()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_check_stop(this));
		}


		private static readonly UvCheckCallback _UvCheckCallback = _CheckCallback;
		private static void _CheckCallback(IntPtr handle)
		{
			var check = FromIntPtr<UvCheck>(handle);
			check._CallbackAction.Invoke(check);
		}
	}
}
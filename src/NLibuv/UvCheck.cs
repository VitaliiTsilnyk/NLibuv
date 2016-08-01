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
		private readonly Action<UvCheck> _Callback;
		private readonly object _State;

		/// <summary>
		/// Gets the stored state object.
		/// </summary>
		public object State => this._State;

		/// <summary>
		/// Initializes a new instance of the <see cref="UvCheck"/> handle.
		/// </summary>
		/// <param name="loop">Loop, on which this handle will be initialized.</param>
		/// <param name="callback">Callback, which will be invoked every loop iteration.</param>
		/// <param name="state">A state object to be stored in the handle for later use inside the callback.</param>
		public UvCheck(UvLoop loop, Action<UvCheck> callback, object state = null)
			: base(loop, UvHandleType.Check)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			this._Callback = callback;
			this._State = state;
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
			check._Callback.Invoke(check);
		}
	}
}
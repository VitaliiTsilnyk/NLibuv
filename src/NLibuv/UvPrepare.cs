using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv prepare handle (uvprepare_t).
	/// Prepare handles will run the given callback once per loop iteration, right before polling for i/o.
	/// </summary>
	public class UvPrepare : UvHandle
	{
		private readonly Action<UvPrepare> _CallbackAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="UvPrepare"/> handle.
		/// </summary>
		/// <param name="loop">Loop, on which this handle will be initialized.</param>
		/// <param name="callbackAction">Callback, which will be invoked every loop iteration.</param>
		public UvPrepare(UvLoop loop, Action<UvPrepare> callbackAction)
			: base(loop, UvHandleType.Prepare)
		{
			if (callbackAction == null)
				throw new ArgumentNullException(nameof(callbackAction));

			this._CallbackAction = callbackAction;
			Libuv.EnsureSuccess(Libuv.uv_prepare_init(loop, this));
			this.NeedsToBeClosed = true;
		}

		/// <summary>
		/// Starts the handle with the callback specified in the constructor.
		/// </summary>
		public void Start()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_prepare_start(this, _UvPrepareCallback));
		}

		/// <summary>
		/// Stop the handle, the callback will no longer be called.
		/// </summary>
		public void Stop()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_prepare_stop(this));
		}


		private static readonly UvPrepareCallback _UvPrepareCallback = _PrepareCallback;
		private static void _PrepareCallback(IntPtr handle)
		{
			var prepare = FromIntPtr<UvPrepare>(handle);
			prepare._CallbackAction.Invoke(prepare);
		}
	}
}
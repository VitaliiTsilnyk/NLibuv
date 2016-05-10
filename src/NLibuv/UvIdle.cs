﻿using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv idle handle (uv_idle_t).
	/// Idle handles will run the given callback once per loop iteration, right before the uv_prepare_t handles.
	/// </summary>
	public class UvIdle : UvHandle
	{
		private readonly Action<UvIdle> _CallbackAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="UvIdle"/> handle.
		/// </summary>
		/// <param name="loop">Loop, on which this handle will be initialized.</param>
		/// <param name="callbackAction">Callback, which will be invoked every loop iteration.</param>
		public UvIdle(UvLoop loop, Action<UvIdle> callbackAction)
			: base(loop, UvHandleType.Idle)
		{
			if (callbackAction == null)
				throw new ArgumentNullException(nameof(callbackAction));

			this._CallbackAction = callbackAction;
			Libuv.EnsureSuccess(Libuv.uv_idle_init(loop, this));
			this.NeedsToBeClosed = true;
		}

		/// <summary>
		/// Starts the handle with the callback specified in the constructor.
		/// </summary>
		public void Start()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_idle_start(this, _UvIdleCallback));
		}

		/// <summary>
		/// Stop the handle, the callback will no longer be called.
		/// </summary>
		public void Stop()
		{
			this.EnsureCallingThread();
			Libuv.EnsureSuccess(Libuv.uv_idle_stop(this));
		}


		private static readonly UvIdleCallback _UvIdleCallback = _IdleCallback;
		private static void _IdleCallback(IntPtr handle)
		{
			var idle = FromIntPtr<UvIdle>(handle);
			idle._CallbackAction.Invoke(idle);
		}
	}
}
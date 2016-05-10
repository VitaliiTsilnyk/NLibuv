using System;
using System.Runtime.InteropServices;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a base libuv request (uv_req_t).
	/// Be careful using requests as they are self-pinned objects and they are not being garbage collected automatically.
	/// </summary>
	/// <typeparam name="TBaseHandle"></typeparam>
	public abstract class UvRequest<TBaseHandle> : UvResourceSafeHandle, IUvRequest
		where TBaseHandle : UvHandle
	{
		private GCHandle _Pin;

		/// <inheritdoc/>
		public UvRequestType RequestType { get; }

		/// <summary>
		/// Gets the handle for which the current request was created.
		/// </summary>
		public TBaseHandle BaseHandle { get; }

		/// <inheritdoc/>
		IUvHandle IUvRequest.BaseHandle => this.BaseHandle;

		/// <summary>
		/// Initializes a new instance of libuv request object.
		/// </summary>
		/// <param name="baseHandle">Base handle for which this request will be executed.</param>
		/// <param name="requestType">Type of the request.</param>
		public UvRequest(TBaseHandle baseHandle, UvRequestType requestType)
			: base(_CalculateSize(requestType), _GetBaseHandleLoopThreadId(baseHandle))
		{
			this.RequestType = requestType;
			this.BaseHandle = baseHandle;

			// Keep this handle alive while request is processing
			this._Pin = GCHandle.Alloc(this, GCHandleType.Normal);
			this.NeedsToBeClosed = true;
		}

		/// <inheritdoc/>
		protected override void CloseHandle()
		{
			if (this._Pin.IsAllocated)
				this._Pin.Free();

			this.Dispose();
		}

		private static int _CalculateSize(UvRequestType requestType)
		{
			return Libuv.uv_req_size(requestType).ToInt32();
		}

		private static int _GetBaseHandleLoopThreadId(IUvHandle baseHandle)
		{
			if (baseHandle == null)
				throw new ArgumentNullException(nameof(baseHandle));

			return baseHandle.Loop.ThreadId;
		}
	}
}
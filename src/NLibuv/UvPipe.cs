using System;
using NLibuv.Native;

namespace NLibuv
{
	/// <summary>
	/// Represents a libuv pipe handle (uv_pipe_t).
	/// Pipe handles provide an abstraction over local domain sockets on Unix and named pipes on Windows.
	/// </summary>
	public class UvPipe : UvNetworkStream
	{
		/// <summary>
		/// Indicates whenever the current pipe was initialized with the IPC option.
		/// </summary>
		public bool Ipc { get; }

		/// <summary>
		/// Initializes a new instance of <see cref="UvPipe"/> handle on the given event loop.
		/// </summary>
		/// <param name="loop"></param>
		/// <param name="ipc">Indicates if this pipe will be used for handle passing between processes.</param>
		public UvPipe(UvLoop loop, bool ipc = false) : base(loop, UvHandleType.NamedPipe)
		{
			this.Ipc = ipc;
			Libuv.EnsureSuccess(Libuv.uv_pipe_init(loop, this, ipc ? 1 : 0));
			this.NeedsToBeClosed = true;
		}

		/// <summary>
		/// Bind the pipe to a file path (Unix) or a name (Windows).
		/// </summary>
		/// <param name="name"></param>
		public void Bind(string name)
		{
			this.EnsureCallingThread();
			
			Libuv.EnsureSuccess(Libuv.uv_pipe_bind(this, name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int PendingCount()
		{
			this.EnsureCallingThread();

			return Libuv.EnsureSuccess(Libuv.uv_pipe_pending_count(this));
		}

		/// <summary>
		/// Connect to the Unix domain socket or the named Windows pipe.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public void Connect(string name, UvPipeConnectRequest.CallbackDelegate callback = null, object state = null)
		{
			this.EnsureCallingThread();

			var request = new UvPipeConnectRequest(this, name, callback, state);
			request.Connect();
		}

		/// <summary>
		/// Extended write function for sending handles over a pipe. The pipe must be initialized with an IPC option.
		/// </summary>
		/// <param name="sendHandle"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		public void WriteHandle(UvStream sendHandle, UvWriteRequest.CallbackDelegate callback, object state)
		{
			this.EnsureCallingThread();

			var request = new UvWriteRequest(this, callback, state);
			request.WriteHandle(sendHandle);
		}
	}
}
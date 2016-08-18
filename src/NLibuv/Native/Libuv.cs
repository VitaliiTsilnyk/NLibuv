using System;
using System.Runtime.InteropServices;


namespace NLibuv.Native
{
	internal static class Libuv
	{

#if LIBUV_INTERNAL
		private const string LibuvName = "__Internal";
#else
		private const string LibuvName = "libuv";
#endif

		/// <summary>
		/// Checks if returned status code is not an error, throws an exception otherwise.
		/// </summary>
		/// <param name="statusCode">Libuv status code.</param>
		/// <returns>The original status code.</returns>
		public static int EnsureSuccess(int statusCode)
		{
			Exception error;
			var result = CheckStatusCode(statusCode, out error);
			if (error != null)
			{
				throw error;
			}
			return result;
		}

		/// <summary>
		/// Checks if returned status code is not an error and tries to create an exception object otherwise.
		/// </summary>
		/// <param name="statusCode">Libuv status code.</param>
		/// <param name="error">Error (if any) or null.</param>
		/// <returns>The original status code.</returns>
		public static int CheckStatusCode(int statusCode, out Exception error)
		{
			error = statusCode < 0 
				? new UvErrorException(statusCode, _GetErrorName(statusCode), _GetErrorDescription(statusCode))
				: null;

			return statusCode;
		}

		private static string _GetErrorName(int err)
		{
			var ptr = uv_err_name(err);
			return ptr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(ptr);
		}

		private static string _GetErrorDescription(int err)
		{
			var ptr = uv_strerror(err);
			return ptr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(ptr);
		}



#region Error handling

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr uv_err_name(int err);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr uv_strerror(int err);

#endregion

#region Common

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern UvBuffer uv_buf_init(IntPtr ptr, int len);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr uv_loop_size();

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr uv_handle_size(UvHandleType type);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr uv_req_size(UvRequestType reqType);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void uv_close(UvHandle handle, UvCloseCallback cb);

#endregion

#region Loop

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_loop_init(UvLoop handle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_loop_close(UvLoop loopHandle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_run(UvLoop handle, UvLoopRunMode mode);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void uv_stop(UvLoop handle);

#endregion

#region Prepare

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_prepare_init(UvLoop loop, UvPrepare idle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_prepare_start(UvPrepare idle, UvPrepareCallback callback);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_prepare_stop(UvPrepare idle);

#endregion

#region Check

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_check_init(UvLoop loop, UvCheck idle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_check_start(UvCheck idle, UvCheckCallback callback);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_check_stop(UvCheck idle);

#endregion

#region Idle

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_idle_init(UvLoop loop, UvIdle idle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_idle_start(UvIdle idle, UvIdleCallback callback);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_idle_stop(UvIdle idle);

#endregion

#region Async

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_async_init(UvLoop loop, UvAsync handle, UvAsyncCallback cb);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_async_send(UvAsync handle);

#endregion

#region Stream

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_shutdown(UvShutdownRequest req, UvStream handle, UvShutdownCallback cb);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_read_start(UvStream handle, UvAllocCallback allocCallback, UvReadCallback readCallback);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_read_stop(UvStream handle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_write(UvWriteRequest req, UvStream handle, UvBuffer[] bufs, int nbufs, UvWriteCallback cb);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_write2(UvWriteRequest req, UvStream handle, UvBuffer[] bufs, int nbufs, UvStream sendHandle, UvWriteCallback cb);

#endregion

#region Network Common

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_listen(UvNetworkStream handle, int backlog, UvConnectionCallback cb);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_accept(UvNetworkStream server, UvStream client);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_ip4_addr(string ip, int port, out SockAddr addr);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_ip6_addr(string ip, int port, out SockAddr addr);

#endregion

#region TCP

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_init(UvLoop loop, UvTcp handle);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_bind(UvTcp handle, ref SockAddr addr, int flags);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_open(UvTcp handle, IntPtr hSocket);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_nodelay(UvTcp handle, int enable);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_connect(UvTcpConnectRequest req, UvTcp handle, ref SockAddr addr, UvConnectCallback cb);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_getsockname(UvTcp handle, out SockAddr name, ref int namelen);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_tcp_getpeername(UvTcp handle, out SockAddr name, ref int namelen);

#endregion

#region Pipe

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_pipe_init(UvLoop loop, UvPipe handle, int ipc);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_pipe_bind(UvPipe loop, string name);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern void uv_pipe_connect(UvPipeConnectRequest req, UvPipe handle, string name, UvConnectCallback cb);

		[DllImport(LibuvName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int uv_pipe_pending_count(UvPipe handle);

#endregion
	}
}
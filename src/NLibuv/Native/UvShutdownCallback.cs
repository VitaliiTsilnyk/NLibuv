using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_shutdown_cb callback function pointer.
	/// </summary>
	/// <param name="req"></param>
	/// <param name="status"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvShutdownCallback(IntPtr req, int status);
}
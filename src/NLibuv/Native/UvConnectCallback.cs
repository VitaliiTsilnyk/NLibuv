using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_connect_cb callback function pointer.
	/// </summary>
	/// <param name="req"></param>
	/// <param name="status"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvConnectCallback(IntPtr req, int status);
}
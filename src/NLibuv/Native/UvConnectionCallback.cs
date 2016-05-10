using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_connection_cb callback function pointer.
	/// </summary>
	/// <param name="server"></param>
	/// <param name="status"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvConnectionCallback(IntPtr server, int status);
}
using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_close_cb callback function pointer.
	/// </summary>
	/// <param name="handle"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvCloseCallback(IntPtr handle);
}
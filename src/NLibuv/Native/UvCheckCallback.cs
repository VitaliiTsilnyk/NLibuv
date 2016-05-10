using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_check_cb callback function pointer.
	/// </summary>
	/// <param name="handle"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvCheckCallback(IntPtr handle);
}
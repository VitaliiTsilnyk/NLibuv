using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_idle_cb callback function pointer.
	/// </summary>
	/// <param name="handle"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvIdleCallback(IntPtr handle);
}
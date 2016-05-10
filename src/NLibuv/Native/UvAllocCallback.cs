using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_alloc_cb callback function pointer.
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="suggestedSize"></param>
	/// <param name="buf"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvAllocCallback(IntPtr handle, int suggestedSize, out UvBuffer buf);
}
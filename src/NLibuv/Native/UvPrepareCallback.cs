using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_prepare_cb callback function pointer.
	/// </summary>
	/// <param name="handle"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvPrepareCallback(IntPtr handle);
}
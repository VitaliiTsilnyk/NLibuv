using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_write_cb callback function pointer.
	/// </summary>
	/// <param name="req"></param>
	/// <param name="status"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvWriteCallback(IntPtr req, int status);
}
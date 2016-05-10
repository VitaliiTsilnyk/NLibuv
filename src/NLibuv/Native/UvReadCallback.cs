using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_read_cb callback function pointer.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="nread"></param>
	/// <param name="buf"></param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void UvReadCallback(IntPtr stream, int nread, ref UvBuffer buf);
}
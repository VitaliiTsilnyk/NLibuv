using System;
using System.Runtime.InteropServices;

namespace NLibuv.Native
{
	/// <summary>
	/// Represents uv_buf_t data structure.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct UvBuffer
	{
		private readonly IntPtr _Field0;
		private readonly IntPtr _Field1;
	}
}